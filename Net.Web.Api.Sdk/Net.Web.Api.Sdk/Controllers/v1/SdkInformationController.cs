using Microsoft.Web.Http;
using Net.Web.Api.Sdk.Common.Constants;
using Net.Web.Api.Sdk.Controllers.Common;
using Net.Web.Api.Sdk.Documentation.Attributes;
using Net.Web.Api.Sdk.Interfaces.Information;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Net.Web.Api.Sdk.Controllers.v1
{
    /// <summary>
    /// Class SdkInformationController.
    /// Implements the <see cref="SdkController" />
    /// </summary>
    /// <seealso cref="SdkController" />
    [EnableCors("*", "*", "*", SupportsCredentials = true)]
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [RoutePrefix(RouteConstants.ROUTE_PREFIX_VERSION)]
    public class SdkInformationController : SdkController
    {
        #region Services

        /// <summary>
        /// The information service
        /// </summary>
        private readonly IInformationService _informationService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SdkInformationController"/> class.
        /// </summary>
        /// <param name="informationService">The information service.</param>
        public SdkInformationController(IInformationService informationService)
        {
            _informationService = informationService ?? throw new ArgumentNullException(nameof(informationService));
        }

        #endregion

        #region Public Services

        /// <summary>
        /// Returns the .Net Web API SDK informations.
        /// </summary>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        [Route("sdk/informations")]
        [AllowAnonymous]
        [SwaggerMethodOrder(1)]
        [SwaggerOperation(Tags = new[] { SwaggerSdkConstants.ABOUT })]
        [SwaggerProduces(ConsumerProducerConstants.JSON)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(JObject))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = ResponseDescriptionConstants.TECHNICAL_ERROR)]
        public IHttpActionResult GetSdkInformations()
        {
            try
            {
                return Ok(_informationService.GetSdkInformations());
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion
    }
}
