using Net.Web.Api.Sdk.Documentation.Attributes;
using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;

namespace Net.Web.Api.Sdk.Documentation.Filters.Common
{
    /// <summary>
    /// Class SwaggerOrderingFilter.
    /// Implements the <see cref="IDocumentFilter" />
    /// </summary>
    /// <seealso cref="IDocumentFilter" />
    public abstract class SwaggerOrderingFilter : IDocumentFilter
    {
        #region Public Virtual Methods

        /// <summary>
        /// Applies the specified swagger document.
        /// </summary>
        /// <param name="swaggerDoc">The swagger document.</param>
        /// <param name="schemaRegistry">The schema registry.</param>
        /// <param name="apiExplorer">The API explorer.</param>
        public virtual void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            OrderingApply(swaggerDoc, schemaRegistry, apiExplorer);
        }

        #endregion

        #region Internal Static Methods

        /// <summary>
        /// Gets the invoke method.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>System.String.</returns>
        internal static string GetInvokeMethod(PathItem item, out string tag)
        {
            tag = string.Empty;

            if (item.get != null)
            {
                tag = item.get.tags != null && item.get.tags.Count == 1 ? item.get.tags[0] : string.Empty;

                return "GET";
            }

            if (item.put != null)
            {
                tag = item.put.tags != null && item.put.tags.Count == 1 ? item.put.tags[0] : string.Empty;

                return "PUT";
            }

            if (item.post != null)
            {
                tag = item.post.tags != null && item.post.tags.Count == 1 ? item.post.tags[0] : string.Empty;

                return "POST";
            }

            if (item.delete != null)
            {
                tag = item.delete.tags != null && item.delete.tags.Count == 1 ? item.delete.tags[0] : string.Empty;

                return "DELETE";
            }

            if (item.options != null)
            {
                tag = item.options.tags != null && item.options.tags.Count == 1 ? item.options.tags[0] : string.Empty;

                return "OPTIONS";
            }

            if (item.head != null)
            {
                tag = item.head.tags != null && item.head.tags.Count == 1 ? item.head.tags[0] : string.Empty;

                return "HEAD";
            }

            if (item.patch == null)
            {
                return string.Empty;
            }

            tag = item.patch.tags != null && item.patch.tags.Count == 1 ? item.patch.tags[0] : string.Empty;

            return "PATCH";
        }

        /// <summary>
        /// Orderings the apply.
        /// </summary>
        /// <param name="swaggerDoc">The swagger document.</param>
        /// <param name="schemaRegistry">The schema registry.</param>
        /// <param name="apiExplorer">The API explorer.</param>
        internal static void OrderingApply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            var paths = swaggerDoc.paths;

            if (paths == null || !paths.Any())
            {
                return;
            }

            var tagGroups = new Dictionary<string, IList<ApiOrder>>();

            foreach (var path in paths)
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

            swaggerDoc.paths = list.OrderBy(c => c.OperationName).ToDictionary(c => c.PathKey, c => c.PathValue);
        }

        /// <summary>
        /// Gets the API order.
        /// </summary>
        /// <param name="apiDescription">The API description.</param>
        /// <returns>System.Int32.</returns>
        internal static int GetApiOrder(ApiDescription apiDescription)
        {
            var apiDescriptor = apiDescription.ActionDescriptor;
            var controllerDescriptor = apiDescriptor?.ControllerDescriptor;

            if (controllerDescriptor == null)
            {
                return -1;
            }

            var controllerType = controllerDescriptor.ControllerType;

            if (controllerType == null)
            {
                return -1;
            }

            var actionName = apiDescriptor.ActionName;

            if (string.IsNullOrEmpty(actionName))
            {
                return -1;
            }

            var actionMethod = controllerType.GetMethod(actionName);

            if (actionMethod == null)
            {
                return -1;
            }

            var attr = actionMethod.GetCustomAttributes(typeof(SwaggerMethodOrderAttribute), false).FirstOrDefault();

            if (attr == null)
            {
                return -1;
            }

            return ((SwaggerMethodOrderAttribute)attr).Order;
        }

        #endregion

        #region Internal Class

        /// <summary>
        /// Class ApiOrder.
        /// </summary>
        internal class ApiOrder
        {
            #region Internal Properties

            /// <summary>
            /// Gets or sets the order.
            /// </summary>
            /// <value>The order.</value>
            internal int Order { get; set; }

            /// <summary>
            /// Gets or sets the path key.
            /// </summary>
            /// <value>The path key.</value>
            internal string PathKey { get; set; }

            /// <summary>
            /// Gets or sets the path value.
            /// </summary>
            /// <value>The path value.</value>
            internal PathItem PathValue { get; set; }

            /// <summary>
            /// Gets or sets the name of the operation.
            /// </summary>
            /// <value>The name of the operation.</value>
            internal string OperationName { get; set; }

            #endregion
        }

        #endregion
    }
}
