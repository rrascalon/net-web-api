using Net.Web.Api.Sdk.Models.Token;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Web.Http.Controllers;

namespace Net.Web.Api.Sdk.Extensions
{
    /// <summary>
    /// Class JwtTokenExtensions.
    /// </summary>
    public static class JwtTokenExtensions
    {
        #region Public Extensions

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns>System.String.</returns>
        public static string GetToken(this HttpActionContext actionContext)
        {
            return GetToken(actionContext.Request.Headers, out _);
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>System.String.</returns>
        public static string GetToken(this HttpRequestMessage httpRequest)
        {
            return GetToken(httpRequest.Headers, out var _);
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <param name="securityToken">The security token.</param>
        /// <returns>System.String.</returns>
        public static string GetToken(this HttpActionContext actionContext, out JwtSecurityToken securityToken)
        {
            return GetToken(actionContext.Request.Headers, out securityToken);
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <param name="securityToken">The security token.</param>
        /// <returns>System.String.</returns>
        public static string GetToken(this HttpRequestMessage httpRequest, out JwtSecurityToken securityToken)
        {
            return GetToken(httpRequest.Headers, out securityToken);
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="rawToken">The raw token.</param>
        /// <param name="securityToken">The security token.</param>
        /// <returns>System.String.</returns>
        public static string GetToken(this string rawToken, out JwtSecurityToken securityToken)
        {
            securityToken = null;

            if (string.IsNullOrEmpty(rawToken))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                securityToken = tokenHandler.ReadToken(rawToken) as JwtSecurityToken;

                return rawToken;
            }
            catch
            {
                try
                {
                    rawToken = rawToken.FromSecuredEncoded64Padding();
                    rawToken = Encoding.UTF8.GetString(Convert.FromBase64String(rawToken));
                    securityToken = tokenHandler.ReadToken(rawToken) as JwtSecurityToken;

                    return rawToken;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the expiration date.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>DateTime.</returns>
        public static DateTime GetExpirationDate(this string token)
        {
            if(string.IsNullOrEmpty(token))
            {
                return DateTime.MinValue;
            }

            GetToken(token, out var jwt);

            if(jwt == null)
            {
                return DateTime.MinValue;
            }

            return jwt.ValidTo;
        }

        /// <summary>
        /// Determines whether [is token one time use] [the specified claims].
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <returns><c>true</c> if [is token one time use] [the specified claims]; otherwise, <c>false</c>.</returns>
        public static bool IsTokenOneTimeUse(this List<Claim> claims)
        {
            if(claims == null || !claims.Any())
            {
                return false;
            }

            var oneTimeUseValue = claims.GetClaimByName(TokenInternalClaimNames.otu.ToString())?.Value;

            return !string.IsNullOrEmpty(oneTimeUseValue) && bool.TryParse(oneTimeUseValue, out var value) && value;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="securityToken">The security token.</param>
        /// <returns>System.String.</returns>
        private static string GetToken(HttpRequestHeaders headers, out JwtSecurityToken securityToken)
        {
            securityToken = null;

            if (string.IsNullOrEmpty(headers.Authorization?.Scheme) || !headers.Authorization.Scheme.Equals("Bearer"))
            {
                return null;
            }

            return headers.Authorization.Parameter.GetToken(out securityToken);
        }

        #endregion
    }
}
