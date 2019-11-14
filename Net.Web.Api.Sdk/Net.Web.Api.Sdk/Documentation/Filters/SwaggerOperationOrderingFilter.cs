using Net.Web.Api.Sdk.Documentation.Attributes;
using Net.Web.Api.Sdk.Documentation.Filters.Common;
using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Http.Description;

namespace Net.Web.Api.Sdk.Documentation.Filters
{
    /// <summary>
    /// Class SwaggerOperationOrderingFilter.
    /// Implements the <see cref="SwaggerOrderingFilter" />
    /// </summary>
    /// <seealso cref="SwaggerOrderingFilter" />
    public class SwaggerOperationOrderingFilter : SwaggerOrderingFilter
    {
        #region IDocumentFilter Implementations


        /// <summary>
        /// Applies the specified swagger document.
        /// </summary>
        /// <param name="swaggerDoc">The swagger document.</param>
        /// <param name="schemaRegistry">The schema registry.</param>
        /// <param name="apiExplorer">The API explorer.</param>
        public override void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            var paths = swaggerDoc.paths;

            if (paths == null || !paths.Any())
            {
                return;
            }

            var allOperationNames = GetOperationOrder(swaggerDoc, apiExplorer);

            if (allOperationNames == null || allOperationNames.Length == 0)
            {
                OrderingApply(swaggerDoc, schemaRegistry, apiExplorer);

                return;
            }

            var groups = new Dictionary<int, IDictionary<string, PathItem>>();

            foreach (var path in paths)
            {
                GetInvokeMethod(path.Value, out var tag);

                var position = allOperationNames.ToList().IndexOf(tag);

                if (position == -1)
                {
                    position = int.MaxValue;
                }

                IDictionary<string, PathItem> dictionary;

                if (groups.ContainsKey(position))
                {
                    dictionary = groups[position];

                    dictionary.Add(path.Key, path.Value);
                }
                else
                {
                    dictionary = new Dictionary<string, PathItem> { { path.Key, path.Value } };

                    groups.Add(position, dictionary);
                }
            }

            groups = ProcessMethodOrdering(groups, apiExplorer);
            groups = groups.OrderBy(c => c.Key).ToDictionary(c => c.Key, c => c.Value);

            var result = new Dictionary<string, PathItem>();

            foreach (var item in groups)
            {
                foreach (var api in item.Value)
                {
                    result.Add(api.Key, api.Value);
                }
            }

            swaggerDoc.paths = result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the operation order.
        /// </summary>
        /// <param name="swaggerDoc">The swagger document.</param>
        /// <param name="apiExplorer">The API explorer.</param>
        /// <returns>System.String[].</returns>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static string[] GetOperationOrder(SwaggerDocument swaggerDoc, IApiExplorer apiExplorer)
        {
            var paths = swaggerDoc.paths;

            if (paths == null || !paths.Any())
            {
                return null;
            }

            var attributes = new List<SwaggerOperationOrderAttribute>();

            foreach (var path in paths)
            {
                var key = GetInvokeMethod(path.Value, out _);
                var apiKey = $"{key}{path.Key.TrimStart('/')}";
                var apiFound = apiExplorer.ApiDescriptions.FirstOrDefault(c => c.ID.StartsWith(apiKey));
                var apiDescriptor = apiFound.ActionDescriptor;
                var controllerDescriptor = apiDescriptor.ControllerDescriptor;
                var controllerType = controllerDescriptor.ControllerType;
                var customAttribute = controllerType.GetCustomAttributes(typeof(SwaggerOperationOrderAttribute), true).FirstOrDefault()
                    as SwaggerOperationOrderAttribute ??
                        controllerType.BaseType.GetCustomAttributes(typeof(SwaggerOperationOrderAttribute), true).FirstOrDefault()
                    as SwaggerOperationOrderAttribute;

                if (customAttribute == null)
                {
                    continue;
                }

                attributes.Add(customAttribute);
            }

            if (!attributes.Any())
            {
                return null;
            }

            var sdk = attributes.FirstOrDefault(c => c.From.Equals(SwaggerOperationOrderAttribute.OperationFrom.Sdk));
            var application = attributes.FirstOrDefault(c => c.From.Equals(SwaggerOperationOrderAttribute.OperationFrom.Application));

            if (application == null && sdk != null)
            {
                return sdk.OperationTags;
            }

            if (sdk == null && application != null)
            {
                return application.OperationTags;
            }

            if (application == null)
            {
                return null;
            }

            var result = sdk.OperationTags.ToList();

            foreach (var cust in application.OperationTags)
            {
                if (result.Contains(cust))
                {
                    continue;
                }

                result.Add(cust);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Processes the item method ordering.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="apiExplorer">The API explorer.</param>
        /// <returns>IDictionary&lt;System.String, PathItem&gt;.</returns>
        private static IDictionary<string, PathItem> ProcessItemMethodOrdering(IDictionary<string, PathItem> group,
            IApiExplorer apiExplorer)
        {
            var tagGroups = new Dictionary<string, IList<ApiOrder>>();

            foreach (var path in group)
            {
                var key = GetInvokeMethod(path.Value, out var tag);
                var apiKey = $"{key}{path.Key.TrimStart('/')}";
                var apiFound = apiExplorer.ApiDescriptions.FirstOrDefault(c => c.ID.StartsWith(apiKey));

                if (!tagGroups.ContainsKey(tag))
                {
                    tagGroups.Add(tag, new List<ApiOrder>());
                }

                var item = new ApiOrder
                {
                    Order = GetApiOrder(apiFound),
                    PathKey = path.Key,
                    PathValue = path.Value,
                    OperationName = tag
                };

                tagGroups[tag].Add(item);
                tagGroups[tag] = tagGroups[tag].OrderBy(c => c.Order).ToList();
            }

            var list = new List<ApiOrder>();

            foreach (var tagGroup in tagGroups)
            {
                list.AddRange(tagGroup.Value);
            }

            return list.OrderBy(c => c.OperationName).ToDictionary(c => c.PathKey, c => c.PathValue);
        }

        /// <summary>
        /// Processes the method ordering.
        /// </summary>
        /// <param name="groups">The groups.</param>
        /// <param name="apiExplorer">The API explorer.</param>
        /// <returns>Dictionary&lt;System.Int32, IDictionary&lt;System.String, PathItem&gt;&gt;.</returns>
        private static Dictionary<int, IDictionary<string, PathItem>> ProcessMethodOrdering(Dictionary<int, IDictionary<string, PathItem>> groups,
            IApiExplorer apiExplorer)
        {
            var result = new Dictionary<int, IDictionary<string, PathItem>>();

            foreach (var item in groups)
            {
                result.Add(item.Key, ProcessItemMethodOrdering(item.Value, apiExplorer));
            }

            return result;
        }

        #endregion
    }
}
