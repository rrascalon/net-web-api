using Microsoft.Web.Http;
using Net.Web.Api.Sdk.Common.Constants;
using Net.Web.Api.Sdk.Controllers.Common;
using Net.Web.Api.Sdk.Documentation.Attributes;
using Net.Web.Api.Sdk.Interfaces.Common;
using Net.Web.Api.Sdk.Models.Services.Common;
using Net.Web.Api.Sdk.Security.Attributes;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Net.Web.Api.Sdk.Controllers.v1
{
    /// <summary>
    /// Class SdkUploadController. This class cannot be inherited.
    /// Implements the <see cref="SdkController" />
    /// </summary>
    /// <seealso cref="SdkController" />
    [EnableCors("*", "*", "*", SupportsCredentials = true)]
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [RoutePrefix(RouteConstants.ROUTE_PREFIX_VERSION)]
    public sealed class SdkUploadController : SdkController
    {
        #region Services


        /// <summary>
        /// The common service
        /// </summary>
        private readonly ICommonService _commonService;

        #endregion

        #region Constructors


        /// <summary>
        /// Initializes a new instance of the <see cref="SdkUploadController"/> class.
        /// </summary>
        /// <param name="commonService">The common service.</param>
        /// <exception cref="ArgumentNullException">commonService</exception>
        public SdkUploadController(ICommonService commonService)
        {
            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));
        }

        #endregion

        #region Public Services

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        [Route("sdk/uploadFile")]
        [TokenAuthorize]
        [SwaggerUploadOperation(typeof(UploadRequest))]
        [SwaggerMethodOrder(1)]
        [SwaggerOperation(Tags = new[] { SwaggerSdkConstants.COMMON })]        
        [SwaggerConsumes(ConsumerProducerConstants.MULTIPART)]
        [SwaggerProduces(ConsumerProducerConstants.JSON)]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(IList<string>), Description = ResponseDescriptionConstants.INVALID_PARAMETER)]     
        [SwaggerResponse(HttpStatusCode.Forbidden, Description = ResponseDescriptionConstants.ACCESS_FORBIDDEN)]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = ResponseDescriptionConstants.AUTHORIZATION_FAILED)]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = ResponseDescriptionConstants.TECHNICAL_ERROR)]

        public IHttpActionResult UploadFile(UploadRequest parameters)
        {
            try
            {
                return Ok(_commonService.UploadFile(
                    parameters.FileInformation.Buffer, 
                    parameters.FileInformation.FileName));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion

    }
}
