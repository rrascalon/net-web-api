using System.Linq;
using System.Web.Http.Description;
using Net.Web.Api.Sdk.Documentation.Attributes;
using Swashbuckle.Swagger;

namespace Net.Web.Api.Sdk.Documentation.Filters
{
    /// <inheritdoc />
    /// <summary>
    /// Class SwaggerUploadOperationFilter.
    /// </summary>
    /// <seealso cref="T:Web.Api.Toolkit.Swashbuckle.Swagger.IOperationFilter" />
    public class SwaggerUploadOperationFilter : IOperationFilter
    {
        #region IOperationFilter Implementations

        /// <inheritdoc />
        /// <summary>
        /// Applies the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="schemaRegistry">The schema registry.</param>
        /// <param name="apiDescription">The API description.</param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var upload = apiDescription.ActionDescriptor.GetCustomAttributes<SwaggerUploadOperationAttribute>().FirstOrDefault();

            if (upload == null)
            {
                return;
            }

            if (!schemaRegistry.Definitions.TryGetValue(upload.ParameterType.Name, out var schema))
            {
                return;
            }

            operation.parameters.Clear();

            foreach (var property in schema.properties)
            {
                var name = property.Key;
                var definition = property.Value;

                if (!string.IsNullOrEmpty(definition.@ref) && definition.@ref.Contains("HttpFile"))
                {
                    operation.parameters.Add(new Parameter
                    {
                        name = name,
                        @in = "formData",
                        description = definition.description,
                        @default = definition.@default,
                        type = "file",
                        required = schema.required.Contains(name)
                    });
                }
                else
                {
                    operation.parameters.Add(new Parameter
                    {
                        name = name,
                        @in = "formData",
                        description = definition.description,
                        @default = definition.@default,
                        type = definition.type,
                        required = schema.required.Contains(name),
                        maxLength = definition.maxLength,
                        minLength = definition.minLength
                    });
                }
            }

            operation.consumes.Add("multipart/form-data");
        }

        #endregion
    }
}
