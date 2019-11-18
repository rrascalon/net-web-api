using Net.Web.Api.Sdk.Controllers.Common;
using Net.Web.Api.Sdk.Documentation.Attributes;
using Net.Web.Api.Sdk.Web.Examples.Classes.Constants;

namespace Net.Web.Api.Sdk.Web.Examples.Controllers.Common
{
    /// <summary>
    /// Class ExampleController.
    /// Implements the <see cref="SdkController" />
    /// </summary>
    /// <seealso cref="SdkController" />
    [SwaggerOperationOrder(From = SwaggerOperationOrderAttribute.OperationFrom.Application,
        OperationTags = new[] {
            ExampleControllerGroups.SECURITY,
            ExampleControllerGroups.OTHER
        })]
    public class ExampleController : SdkController {
        #region Constants

        /// <summary>
        /// The route prefix
        /// </summary>
        protected const string ROUTE_PREFIX = "example/";

        #endregion
    }
}