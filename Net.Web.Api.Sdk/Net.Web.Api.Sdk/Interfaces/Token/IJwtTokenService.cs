using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Http.Controllers;
using Microsoft.IdentityModel.Tokens;
using Net.Web.Api.Sdk.Injection.Attributes;
using Net.Web.Api.Sdk.Models.Token;

namespace Net.Web.Api.Sdk.Interfaces.Token
{
    /// <summary>
    /// Interface IJwtTokenService
    /// </summary>
    [InjectInterfaceService]
    public interface IJwtTokenService
    {
        /// <summary>
        /// Gets the tokens.
        /// </summary>
        /// <value>The tokens.</value>
        Dictionary<string, JwtTokenModel> Tokens { get; }

        /// <summary>
        /// Creates the token.
        /// </summary>
        /// <param name="tokenName">Name of the token.</param>
        /// <param name="identityName">Name of the identity.</param>
        /// <param name="customClaims">The custom claims.</param>
        /// <returns>System.String.</returns>
        string CreateToken(string tokenName, string identityName, Dictionary<string, string> customClaims = null);

        /// <summary>
        /// Gets the token validation parameters.
        /// </summary>
        /// <param name="validateExipration">if set to <c>true</c> [validate exipration].</param>
        /// <param name="issuers">The issuers.</param>
        /// <param name="audiences">The audiences.</param>
        /// <returns>TokenValidationParameters.</returns>
        TokenValidationParameters GetTokenValidationParameters(bool validateExipration = false, string issuers = null, string audiences = null);

        /// <summary>
        /// Gets the token payload.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        Dictionary<string, string> GetTokenPayload(HttpActionContext context);

        /// <summary>
        /// Gets the identity payload.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        Dictionary<string, string> GetIdentityPayload(HttpActionContext context);

        /// <summary>
        /// Determines whether [is token revoked] [the specified token].
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="claims">The claims.</param>
        /// <returns><c>true</c> if [is token revoked] [the specified token]; otherwise, <c>false</c>.</returns>
        bool IsTokenRevoked(string token, List<Claim> claims);

        /// <summary>
        /// Determines whether [is token used] [the specified token].
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="claims">The claims.</param>
        /// <returns><c>true</c> if [is token used] [the specified token]; otherwise, <c>false</c>.</returns>
        bool IsTokenUsed(string token, List<Claim> claims);

        /// <summary>
        /// Revokes the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="claims">The claims.</param>
        bool RevokeToken(string token, List<Claim> claims);

        /// <summary>
        /// Marks the token as used.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="claims">The claims.</param>
        void MarkTokenAsUsed(string token, List<Claim> claims);

        /// <summary>
        /// Cleanups the token database.
        /// </summary>
        /// <returns>System.Int32.</returns>
        int CleanupTokenDatabase();
    }
}
