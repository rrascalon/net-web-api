using Microsoft.Web.Http;
using Net.Web.Api.Sdk.Common.Constants;
using Net.Web.Api.Sdk.Controllers.Common;
using Net.Web.Api.Sdk.Documentation.Attributes;
using Net.Web.Api.Sdk.Extensions;
using Net.Web.Api.Sdk.Interfaces.Token;
using Net.Web.Api.Sdk.Models.Services.Token;
using Net.Web.Api.Sdk.Security.Attributes;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Net.Web.Api.Sdk.Controllers.v1
{
    /// <summary>
    /// Class SdkTokenController.
    /// Implements the <see cref="SdkController" />
    /// </summary>
    /// <seealso cref="SdkController" />
    [EnableCors("*", "*", "*", SupportsCredentials = true)]
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [RoutePrefix(RouteConstants.ROUTE_PREFIX_VERSION)]
    public class SdkTokenController : SdkController
    {
        #region Services

        /// <summary>
        /// The token service
        /// </summary>
        private readonly IJwtTokenService _tokenService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SdkTokenController"/> class.
        /// </summary>
        /// <param name="tokenService">The token service.</param>
        /// <exception cref="ArgumentNullException">tokenService</exception>
        public SdkTokenController(IJwtTokenService tokenService)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        #endregion

        #region Public Services

        /// <summary>
        /// Creates a new JWT Token.
        /// </summary>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        [Route("sdk/createToken")]
        [AllowAnonymous]
        [SwaggerMethodOrder(1)]
        [SwaggerOperation(Tags = new[] { SwaggerSdkConstants.SECURITY })]
        [SwaggerProduces(ConsumerProducerConstants.JSON)]
        [SwaggerConsumes(ConsumerProducerConstants.JSON)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CreateTokenResult))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(IList<string>), Description = ResponseDescriptionConstants.INVALID_PARAMETER)]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = ResponseDescriptionConstants.TECHNICAL_ERROR)]
        public IHttpActionResult CreateToken(CreateTokenRequest paramaters)
        {
            try
            {
                return Ok(
                    new CreateTokenResult(
                        _tokenService.CreateToken(
                            paramaters.Name, 
                            paramaters.UniqueId, 
                            paramaters.Payload != null ? paramaters.Payload.ToDictionary(c=>c.Key, c=>c.Value) : null
                        )
                    )
                );
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Revokes a token passed in the authorization header.
        /// </summary>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        [Route("sdk/revokeToken")]
        [TokenAuthorize]
        [SwaggerMethodOrder(2)]
        [SwaggerOperation(Tags = new[] { SwaggerSdkConstants.SECURITY })]
        [SwaggerProduces(ConsumerProducerConstants.JSON)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.Forbidden, Description = ResponseDescriptionConstants.ACCESS_FORBIDDEN)]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = ResponseDescriptionConstants.AUTHORIZATION_FAILED)]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = ResponseDescriptionConstants.TECHNICAL_ERROR)]
        public IHttpActionResult RevokeToken()
        {
            try
            {
                var token = ActionContext.GetToken();
                var claims = this.GetClaims().ToList(); ;

                return Ok(_tokenService.RevokeToken(token, claims));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Validates a token passed in the authorization header.
        /// </summary>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        [Route("sdk/validateToken")]
        [TokenAuthorize]
        [SwaggerMethodOrder(3)]
        [SwaggerOperation(Tags = new[] { SwaggerSdkConstants.SECURITY })]
        [SwaggerProduces(ConsumerProducerConstants.JSON)]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Forbidden, Description = ResponseDescriptionConstants.ACCESS_FORBIDDEN)]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = ResponseDescriptionConstants.AUTHORIZATION_FAILED)]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = ResponseDescriptionConstants.TECHNICAL_ERROR)]
        public IHttpActionResult ValidateToken()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion
    }
}
