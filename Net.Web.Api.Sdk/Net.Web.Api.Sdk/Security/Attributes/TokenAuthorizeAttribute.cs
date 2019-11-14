using Microsoft.IdentityModel.Tokens;
using Net.Web.Api.Sdk.Extensions;
using Net.Web.Api.Sdk.Injection.Containers;
using Net.Web.Api.Sdk.Interfaces.Token;
using Net.Web.Api.Sdk.Models.Token;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Net.Web.Api.Sdk.Security.Attributes
{
    /// <summary>
    /// Class TokenAuthorizeAttribute.
    /// Implements the <see cref="AuthorizeAttribute" />
    /// </summary>
    /// <seealso cref="AuthorizeAttribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TokenAuthorizeAttribute : AuthorizeAttribute
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the issuers.
        /// </summary>
        /// <value>The issuers.</value>
        public string Issuers { get; set; }

        /// <summary>
        /// Gets or sets the intended audiences.
        /// </summary>
        /// <value>The intended audiences.</value>
        public string IntendedAudiences { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [validate expiration].
        /// </summary>
        /// <value><c>true</c> if [validate expiration]; otherwise, <c>false</c>.</value>
        public bool ValidateExpiration { get; set; }

        /// <summary>
        /// Gets or sets the name of the token validating.
        /// </summary>
        /// <value>The name of the token validating.</value>
        public string TokenValidatingName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenAuthorizeAttribute"/> class.
        /// </summary>
        public TokenAuthorizeAttribute() => ValidateExpiration = true;

        #endregion

        #region AuthorizeAttribute Overrides

        /// <summary>
        /// Called when [authorization asynchronous].
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var identity = actionContext.RequestContext.Principal.Identity;

            if (identity == null || !identity.IsAuthenticated)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, TokenStatus.TokenRequired);

                return Task.FromResult<object>(null);
            }

            var token = actionContext.GetToken();

            if (string.IsNullOrEmpty(token))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, TokenStatus.TokenRequired);

                return Task.FromResult<object>(null);
            }

            var claims = ((ClaimsIdentity)identity).Claims.ToList();

            if (string.IsNullOrEmpty(TokenValidatingName))
            {
                TokenValidatingName = claims.GetClaimByName(TokenInternalClaimNames.tn.ToString())?.Value;
            }

            if (string.IsNullOrEmpty(TokenValidatingName))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, TokenStatus.Invalid);

                return Task.FromResult<object>(null);
            }

            var service = InjectionContainer.Instance.GetService<IJwtTokenService>();
            var tokenDefinition = service.Tokens.ContainsKey(TokenValidatingName) ? service.Tokens[TokenValidatingName] : null;

            if (tokenDefinition == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, TokenStatus.Invalid);

                return Task.FromResult<object>(null);
            }

            try
            {
                var validationParameters = service.GetTokenValidationParameters(ValidateExpiration, Issuers, IntendedAudiences);
                var tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(token, validationParameters, out _);

                var isRevoked = service.IsTokenRevoked(token, claims);

                if(isRevoked)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, TokenStatus.Revoked);

                    return Task.FromResult<object>(null);
                }

                if (claims.IsTokenOneTimeUse())
                {
                    var isUsed = service.IsTokenUsed(token, claims);

                    if (isUsed)
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, TokenStatus.AlreadyUsed);

                        return Task.FromResult<object>(null);
                    }

                    service.MarkTokenAsUsed(token, claims);
                }

                return Task.FromResult<object>(null);
            }
            catch(SecurityTokenExpiredException)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, TokenStatus.Expired);

                return Task.FromResult<object>(null);
            }
            catch (SecurityTokenInvalidAudienceException)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, TokenStatus.InvalidAudience);

                return Task.FromResult<object>(null);
            }
            catch (Exception ex)
            {
                var isTechnicalError = !(ex.Message.StartsWith("IDX") && ex.Message.Contains(":"));

                actionContext.Response = isTechnicalError 
                    ? actionContext.Request.CreateResponse(HttpStatusCode.InternalServerError, ex)
                    : actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, TokenStatus.Invalid);

                return Task.FromResult<object>(null);
            }
        }

        #endregion
    }
}
