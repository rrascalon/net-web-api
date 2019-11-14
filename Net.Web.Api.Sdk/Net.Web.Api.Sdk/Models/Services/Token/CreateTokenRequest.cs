using Net.Web.Api.Sdk.Attributes.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Net.Web.Api.Sdk.Models.Services.Token
{
    /// <summary>
    /// Class CreateTokenRequest.
    /// </summary>
    public class CreateTokenRequest
    {
        #region Public Properties

        /// <summary>
        /// The token name as defined in the token configuration files.
        ///     - Case insensitive.
        /// </summary>
        /// <value>The name.</value>
        [Required]
        [StringLength(16)]
        [TokenNameExists]
        public string Name { get; set; }

        /// <summary>
        /// The token unique identifier that will be used as identity name.
        ///     - User name,
        ///     - Email address,
        ///     - Any other unique identifiers.
        /// </summary>
        /// <value>The unique identifier.</value>
        [Required]
        [StringLength(64)]
        public string UniqueId { get; set; }

        /// <summary>
        /// The token payload.
        ///     - keys cannot be empty.
        ///     - keys are to be unique.
        ///     - values cannot be empty.
        /// </summary>
        /// <value>The payload.</value>
        [TokenPayloadValid]
        public List<KeyValuePair<string, string>> Payload { get; set; }

        #endregion
    }
}
