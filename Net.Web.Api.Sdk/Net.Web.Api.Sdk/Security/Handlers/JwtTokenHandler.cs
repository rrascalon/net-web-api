using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Net.Web.Api.Sdk.Extensions;
using Net.Web.Api.Sdk.Injection.Containers;
using Net.Web.Api.Sdk.Interfaces.Token;

namespace Net.Web.Api.Sdk.Security.Handlers
{
    /// <inheritdoc />
    /// <summary>
    /// Class JwtTokenHandler.
    /// </summary>
    /// <seealso cref="T:System.Net.Http.DelegatingHandler" />
    public class JwtTokenHandler : DelegatingHandler
    {
        #region DelegatingHandler Overrides

        /// <inheritdoc />
        /// <summary>
        /// send as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var token = request.GetToken(out var securityToken);

                if (string.IsNullOrEmpty(token) || securityToken == null)
                {
                    return await base.SendAsync(request, cancellationToken);
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var service = InjectionContainer.Instance.GetService<IJwtTokenService>();
                var principal = tokenHandler.ValidateToken(token, service.GetTokenValidationParameters(), out _);

                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = principal;
                }

                Thread.CurrentPrincipal = principal;

                return await base.SendAsync(request, cancellationToken);
            }
            catch
            {
                // ignored
            }

            return await base.SendAsync(request, cancellationToken);
        }

        #endregion
    }
}
