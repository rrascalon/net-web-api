using Net.Web.Api.Sdk.Common.Constants;
using Net.Web.Api.Sdk.Documentation.Attributes;
using System.Web.Http;

namespace Net.Web.Api.Sdk.Controllers.Common
{
    /// <summary>
    /// Class SdkController.
    /// Implements the <see cref="ApiController" />
    /// </summary>
    /// <seealso cref="ApiController" />
    [SwaggerOperationOrder(From = SwaggerOperationOrderAttribute.OperationFrom.Sdk,
        OperationTags = new[] {
            SwaggerSdkConstants.ABOUT,
            SwaggerSdkConstants.SECURITY,
            SwaggerSdkConstants.COMMON
        })]
    public class SdkController : ApiController {}
}
