using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace Net.Web.Api.Sdk.Extensions
{
    /// <summary>
    /// Class ControllerExtensions.
    /// </summary>
    public static class ControllerExtensions
    {
        #region Public Extensions

        /// <summary>
        /// Gets the claims.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns>IList&lt;Claim&gt;.</returns>
        public static IList<Claim> GetClaims(this ApiController controller)
        {
            var identity = controller.ActionContext.RequestContext.Principal.Identity;

            if (identity == null || !identity.IsAuthenticated)
            {
                return null;
            }

            return ((ClaimsIdentity)identity).Claims.ToList();
        }

        #endregion
    }
}
