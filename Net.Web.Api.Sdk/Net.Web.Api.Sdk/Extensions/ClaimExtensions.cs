using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Net.Web.Api.Sdk.Extensions
{
    /// <summary>
    /// Class ClaimExtensions.
    /// </summary>
    public static class ClaimExtensions
    {
        #region Extension Methods

        /// <summary>
        /// Gets the name of the claim by.
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <param name="name">The name.</param>
        /// <returns>Claim.</returns>
        public static Claim GetClaimByName(this IList<Claim> claims, string name)
        {
            return claims.FirstOrDefault(c => c.Type.Equals(name));
        }

        /// <summary>
        /// To the claim dictionary.
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <param name="shortNameOnly">if set to <c>true</c> [short name only].</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public static Dictionary<string, string> ToClaimDictionary(this IEnumerable<Claim> claims, bool shortNameOnly = false)
        {
            return claims?.DistinctBy(c=>c.Type).ToDictionary(claim => claim.Type.ShortName(shortNameOnly), claim => claim.Value) 
                   ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Shorts the name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="shortNameOnly">if set to <c>true</c> [short name only].</param>
        /// <returns>System.String.</returns>
        private static string ShortName(this string name, bool shortNameOnly = false)
        {
            if (!shortNameOnly)
            {
                return name;
            }

            var items = name.Split('/');

            return items[items.Length - 1];
        }

        #endregion
    }
}
