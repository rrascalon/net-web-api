using System.Configuration;

namespace Net.Web.Api.Sdk.Configurations.Token
{
    /// <inheritdoc />
    /// <summary>
    /// Class TokenDefinitionElement.
    /// </summary>
    /// <seealso cref="T:System.Configuration.ConfigurationElement" />
    public class TokenDefinitionElement : ConfigurationElement
    {
        #region Private Constants

        /// <summary>
        /// The issuer
        /// </summary>
        private const string ISSUER = "issuer";

        /// <summary>
        /// The intended audience
        /// </summary>
        private const string INTENDED_AUDIENCE = "intendedAudience";

        /// <summary>
        /// The expiration in minute
        /// </summary>
        private const string EXPIRATION_IN_MINUTE = "expirationInMinute";

        /// <summary>
        /// The is base 64 encoded
        /// </summary>
        private const string IS_BASE_64_ENCODED = "isBase64Encoded";

        /// <summary>
        /// The one time use
        /// </summary>
        private const string ONE_TIME_USE = "oneTimeUse";

        /// <summary>
        /// The signature
        /// </summary>
        private const string SIGNATURE = "signature";

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        /// <value>The issuer.</value>
        [ConfigurationProperty(ISSUER, DefaultValue = "http://no.where.com", IsKey = false, IsRequired = true)]
        public string Issuer
        {
            get => (string)base[ISSUER];

            set => base[ISSUER] = value;
        }

        /// <summary>
        /// Gets or sets the intended audience.
        /// </summary>
        /// <value>The intended audience.</value>
        [ConfigurationProperty(INTENDED_AUDIENCE, DefaultValue = "urn:no-where", IsKey = false, IsRequired = true)]
        public string IntendedAudience
        {
            get => (string)base[INTENDED_AUDIENCE];

            set => base[INTENDED_AUDIENCE] = value;
        }

        /// <summary>
        /// Gets or sets the expiration in minute.
        /// </summary>
        /// <value>The expiration in minute.</value>
        [ConfigurationProperty(EXPIRATION_IN_MINUTE, DefaultValue = 15d, IsKey = false, IsRequired = true)]
        public double ExpirationInMinute
        {
            get => (double)base[EXPIRATION_IN_MINUTE];

            set => base[EXPIRATION_IN_MINUTE] = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is base64 encoded.
        /// </summary>
        /// <value><c>true</c> if this instance is base64 encoded; otherwise, <c>false</c>.</value>
        [ConfigurationProperty(IS_BASE_64_ENCODED, DefaultValue = false, IsKey = false, IsRequired = false)]
        public bool IsBase64Encoded
        {
            get => (bool)base[IS_BASE_64_ENCODED];

            set => base[IS_BASE_64_ENCODED] = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [one time use].
        /// </summary>
        /// <value><c>true</c> if [one time use]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty(ONE_TIME_USE, DefaultValue = false, IsKey = false, IsRequired = false)]
        public bool OneTimeUse
        {
            get => (bool)base[ONE_TIME_USE];

            set => base[ONE_TIME_USE] = value;
        }

        /// <summary>
        /// Gets the signature.
        /// </summary>
        /// <value>The signature.</value>
        [ConfigurationProperty(SIGNATURE, IsRequired = true)]
        public TokenSignatureElement Signature => (TokenSignatureElement)this[SIGNATURE];

        #endregion
    }
}
