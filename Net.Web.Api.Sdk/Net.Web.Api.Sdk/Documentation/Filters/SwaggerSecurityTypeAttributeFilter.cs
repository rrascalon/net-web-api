using Net.Web.Api.Sdk.Documentation.Attributes;
using Net.Web.Api.Sdk.Documentation.Constants;
using Net.Web.Api.Sdk.Security.Attributes;
using Swashbuckle.Swagger;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace Net.Web.Api.Sdk.Documentation.Filters
{
    /// <inheritdoc />
    /// <summary>
    /// Class SwaggerSecurityTypeAttributeFilter.
    /// </summary>
    public class SwaggerSecurityTypeAttributeFilter : IOperationFilter
    {
        #region IOperationFilter Implementation

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
            var securityType = GetSecurityType(apiDescription);

            if (!string.IsNullOrEmpty(securityType))
            {
                operation.description = securityType;
            }
            else
            {
                var attr = apiDescription.GetControllerAndActionAttributes<SwaggerSecurityTypeAttribute>().FirstOrDefault();

                if (attr != null)
                {
                    operation.description = attr.SecurityType;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the type of the security.
        /// </summary>
        /// <param name="apiDescription">The API description.</param>
        /// <returns>System.String.</returns>
        private static string GetSecurityType(ApiDescription apiDescription)
        {
            var actionDescription = apiDescription.ActionDescriptor;
            var controllerDescriptor = actionDescription?.ControllerDescriptor;

            if (controllerDescriptor == null)
            {
                return null;
            }

            var controllerType = controllerDescriptor.ControllerType;

            if (controllerType == null)
            {
                return null;
            }

            var actionName = actionDescription.ActionName;

            if (string.IsNullOrEmpty(actionName))
            {
                return null;
            }

            var actionMethod = controllerType.GetMethod(actionName);

            if (actionMethod == null)
            {
                return null;
            }

            var customAttribute = actionMethod.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).FirstOrDefault();

            if (customAttribute != null)
            {
                return SwaggerSecurityTypeConstants.ANONYMOUS;
            }

            customAttribute = actionMethod.GetCustomAttributes(typeof(TokenAuthorizeAttribute), true).FirstOrDefault();

            if (customAttribute != null)
            {
                return SwaggerSecurityTypeConstants.TOKEN_SECURED + GetIntendedAudiences(customAttribute);
            }

            customAttribute = actionMethod.GetCustomAttributes(typeof(BasicAuthorizeAttribute), true).FirstOrDefault();

            if (customAttribute != null)
            {
                return SwaggerSecurityTypeConstants.BASIC_SECURED;
            }

            customAttribute = controllerType.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).FirstOrDefault();

            if (customAttribute != null)
            {
                return SwaggerSecurityTypeConstants.ANONYMOUS;
            }

            customAttribute = controllerType.GetCustomAttributes(typeof(TokenAuthorizeAttribute), true).FirstOrDefault();

            return customAttribute != null
                ? SwaggerSecurityTypeConstants.TOKEN_SECURED + GetIntendedAudiences(customAttribute)
                : SwaggerSecurityTypeConstants.ANONYMOUS;
        }

        /// <summary>
        /// Gets the intended audiences.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string GetIntendedAudiences(object attribute)
        {
            if (!(attribute is TokenAuthorizeAttribute tokenAuthorizeAttribute))
            {
                return string.Empty;
            }

            var audienceString = tokenAuthorizeAttribute.IntendedAudiences;

            return string.IsNullOrEmpty(audienceString) ? string.Empty : $"<br/>Intended Audience(s): {audienceString}";
        }

        #endregion
    }
}
