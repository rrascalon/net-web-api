using System;

namespace Net.Web.Api.Sdk.Models.Token
{
    /// <summary>
    /// Class JwtTokenUsedOrRevoked.
    /// </summary>
    public class JwtTokenUsedOrRevoked
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>The token.</value>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is used.
        /// </summary>
        /// <value><c>true</c> if this instance is used; otherwise, <c>false</c>.</value>
        public bool IsUsed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is revoked.
        /// </summary>
        /// <value><c>true</c> if this instance is revoked; otherwise, <c>false</c>.</value>
        public bool IsRevoked { get; set; }

        /// <summary>
        /// Gets or sets the revocation date.
        /// </summary>
        /// <value>The revocation date.</value>
        public DateTime? RevocationDate { get; set; }

        /// <summary>
        /// Gets or sets the used date.
        /// </summary>
        /// <value>The used date.</value>
        public DateTime? UsedDate { get; set; }

        /// <summary>
        /// Gets or sets the expiration date.
        /// </summary>
        /// <value>The expiration date.</value>
        public DateTime ExpirationDate { get; set; }

        #endregion
    }
}
