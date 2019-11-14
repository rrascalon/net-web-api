using System.Configuration;

namespace Net.Web.Api.Sdk.Configurations.Token
{
    /// <inheritdoc />
    /// <summary>
    /// Class TokenSecurityElement.
    /// </summary>
    /// <seealso cref="T:System.Configuration.ConfigurationElement" />
    public class TokenSignatureElement : ConfigurationElement
    {
        #region Private Constants

        /// <summary>
        /// The pass phrase
        /// </summary>
        private const string PASS_PHRASE = "passPhrase";

        /// <summary>
        /// The signing certificate
        /// </summary>
        private const string SIGNING_CERTIFICATE = "signingCertificate";

        /// <summary>
        /// The signing certificate password
        /// </summary>
        private const string SIGNING_CERTIFICATE_PASSWORD = "signingCertificatePassword";

        /// <summary>
        /// The validating certificate
        /// </summary>
        private const string VALIDATING_CERTIFICATE = "validatingCertificate";

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the pass phrase.
        /// </summary>
        /// <value>The pass phrase.</value>
        [ConfigurationProperty(PASS_PHRASE, DefaultValue = "", IsKey = false, IsRequired = false)]
        public string PassPhrase
        {
            get => (string)base[PASS_PHRASE];

            set => base[PASS_PHRASE] = value;
        }

        /// <summary>
        /// Gets or sets the signing certificate.
        /// </summary>
        /// <value>The signing certificate.</value>
        [ConfigurationProperty(SIGNING_CERTIFICATE, DefaultValue = "", IsKey = false, IsRequired = false)]
        public string SigningCertificate
        {
            get => (string)base[SIGNING_CERTIFICATE];

            set => base[SIGNING_CERTIFICATE] = value;
        }

        /// <summary>
        /// Gets or sets the signing certificate password.
        /// </summary>
        /// <value>The signing certificate password.</value>
        [ConfigurationProperty(SIGNING_CERTIFICATE_PASSWORD, DefaultValue = "", IsKey = false, IsRequired = false)]
        public string SigningCertificatePassword
        {
            get => (string)base[SIGNING_CERTIFICATE_PASSWORD];

            set => base[SIGNING_CERTIFICATE_PASSWORD] = value;
        }

        /// <summary>
        /// Gets or sets the validating certificate.
        /// </summary>
        /// <value>The validating certificate.</value>
        [ConfigurationProperty(VALIDATING_CERTIFICATE, DefaultValue = "", IsKey = false, IsRequired = false)]
        public string ValidatingCertificate
        {
            get => (string)base[VALIDATING_CERTIFICATE];

            set => base[VALIDATING_CERTIFICATE] = value;
        }
        
        #endregion
    }
}
