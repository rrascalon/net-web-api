using Net.Web.Api.Sdk.Documentation.Attributes;
using Swashbuckle.Swagger;
using System.Linq;
using System.Web.Http.Description;

namespace Net.Web.Api.Sdk.Documentation.Filters
{
    /// <inheritdoc />
    /// <summary>
    /// Class SwaggerProducesFilter.
    /// </summary>
    public class SwaggerProducesFilter : IOperationFilter
    {
        #region IOperationFilter Implementations

        /// <inheritdoc />
        /// <summary>
        /// Applies the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="schemaRegistry">The schema registry.</param>
        /// <param name="apiDescription">The API description.</param>
        /// <exception cref="T:System.NotImplementedException"></exception>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var attribute = apiDescription.GetControllerAndActionAttributes<SwaggerProducesAttribute>().SingleOrDefault();

            if (attribute == null)
            {
                return;
            }

            operation.produces.Clear();
            operation.produces = attribute.ContentTypes.ToList();
        }

        #endregion
    }
}
