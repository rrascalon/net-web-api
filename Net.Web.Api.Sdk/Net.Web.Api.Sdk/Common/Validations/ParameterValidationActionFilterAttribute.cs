using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Net.Web.Api.Sdk.Common.Validations
{
    /// <summary>
    /// Class ParameterValidationActionFilterAttribute.
    /// Implements the <see cref="ActionFilterAttribute" />
    /// </summary>
    /// <seealso cref="ActionFilterAttribute" />
    public class ParameterValidationActionFilterAttribute : ActionFilterAttribute
    {
        #region Private Properties

        /// <summary>
        /// The formatter
        /// </summary>
        private readonly JsonMediaTypeFormatter _formatter;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterValidationActionFilterAttribute"/> class.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        public ParameterValidationActionFilterAttribute(JsonMediaTypeFormatter formatter)
        {
            _formatter = formatter;
        }

        #endregion

        #region ActionFilterAttribute Overrides

        /// <inheritdoc />
        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest,
                    CreateValidationErrorContent(actionContext), _formatter);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the content of the validation error.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>JArray.</returns>
        private static JArray CreateValidationErrorContent(HttpActionContext actionContext)
        {
            var result = (
                from value 
                in actionContext.ModelState.Values 
                from message 
                in value.Errors 
                select message.ErrorMessage).Where(c=>!string.IsNullOrEmpty(c));

            return JArray.FromObject(result.Distinct().ToList());
        }

        #endregion
    }
}
