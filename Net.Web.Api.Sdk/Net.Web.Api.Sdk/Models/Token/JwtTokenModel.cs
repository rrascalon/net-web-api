using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.IdentityModel.Tokens;
using Net.Web.Api.Sdk.Configurations.Token;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Net.Web.Api.Sdk.Models.Token
{
    internal enum TokenInternalClaimNames
    {
        /// <summary>
        /// The token will not be valid before a give utc date/time
        /// </summary>
        nbf,

        /// <summary>
        /// The token expiration utc date/time
        /// </summary>
        exp,

        /// <summary>
        /// The token issue utc date/time
        /// </summary>
        iat,

        /// <summary>
        /// The token issuer
        /// </summary>
        iss,

        /// <summary>
        /// The token audince
        /// </summary>
        aud,

        /// <summary>
        /// The token unique identifier
        /// </summary>
        jti,

        /// <summary>
        /// The the token expiration in minutes
        /// </summary>
        exm,

        /// <summary>
        /// The token name
        /// </summary>
        tn,

        /// <summary>
        /// One time use token
        /// </summary>
        otu
    }

    /// <summary>
    /// Enum TokenSecurityType
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TokenSecurityTypes
    {
        /// <summary>
        /// The pass phrase
        /// </summary>
        PassPhrase,

        /// <summary>
        /// The certificate
        /// </summary>
        Certificate
    }

    /// <summary>
    /// Enum TokenSecurityAlgorithms
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TokenSecurityAlgorithms
    {
        /// <summary>
        /// The hmac sha256
        /// </summary>
        HmacSha256,

        /// <summary>
        /// The hmac sha384
        /// </summary>
        HmacSha384,

        /// <summary>
        /// The hmac sha512
        /// </summary>
        HmacSha512
    }

    /// <summary>
    /// Enum TokenStatus
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TokenStatus
    {
        /// <summary>
        /// The already used
        /// </summary>
        AlreadyUsed,

        /// <summary>
        /// The expired
        /// </summary>
        Expired,

        /// <summary>
        /// The invalid
        /// </summary>
        Invalid,

        /// <summary>
        /// The invalid audience
        /// </summary>
        InvalidAudience,

        /// <summary>
        /// The required
        /// </summary>
        TokenRequired,

        /// <summary>
        /// The revoked
        /// </summary>
        Revoked,

        /// <summary>
        /// The valid
        /// </summary>
        Valid
    }

    /// <summary>
    /// Interface ITokenCredential
    /// </summary>
    internal interface ITokenCredential
    {
        /// <summary>
        /// Gets or sets the security key.
        /// </summary>
        /// <value>The security key.</value>
        SecurityKey SecurityKey { get; set; }

        /// <summary>
        /// Gets or sets the signing credentials.
        /// </summary>
        /// <value>The signing credentials.</value>
        SigningCredentials SigningCredentials { get; set; }
    }

    /// <summary>
    /// Class JwtTokenModel.
    /// </summary>
    public class JwtTokenModel
    {
        #region Private Constants

        /// <summary>
        /// The urn pattern
        /// </summary>
        private const string URN_PATTERN = @"^(?<URN>[uU][rR][nN]\:(?<NID>(?!urn\:)[a-zA-Z0-9][a-zA-Z0-9-]{1,31})\:(?<NSS>([a-zA-Z0-9()+,._!*':=@;$-]|%[0-9a-fA-F]{2})+))$";

        #endregion

        #region Public Properties

        /// <summary>
        /// The token name:
        ///     - Case insensitive.
        /// </summary>
        /// <value>The name of the token.</value>
        public string TokenName { get; }

        /// <summary>
        /// The token Issuer:
        ///     - URL Format.
        /// </summary>
        /// <value>The token issuer.</value>
        public string TokenIssuer { get; }

        /// <summary>
        /// The token intended audience:
        ///     - URN Format.
        /// </summary>
        /// <value>The token intended audience.</value>
        public string TokenIntendedAudience { get; }

        /// <summary>
        /// The token expiration in minutes.
        /// </summary>
        /// <value>The token expiration in minutes.</value>
        public double TokenExpirationInMinutes { get; }

        /// <summary>
        /// Specifies if the resulting token will be base 64 encoded:
        ///     - In order to use the token not only in the request header but also as request parameters:
        ///         - If the result encoded token is padded with = signs, these signs will bew replace by [EQUAL]
        /// </summary>
        /// <value><c>true</c> if this instance is token base64 encoded; otherwise, <c>false</c>.</value>
        public bool IsTokenBase64Encoded { get; }

        /// <summary>
        /// Gets a value indicating whether [one time use].
        /// </summary>
        /// <value><c>true</c> if [one time use]; otherwise, <c>false</c>.</value>
        public bool OneTimeUse { get; }

        /// <summary>
        /// The token security type:
        ///     - PassPhrase
        ///     - Certificate
        /// </summary>
        /// <value>The type of the token security.</value>       
        public TokenSecurityTypes TokenSecurityType { get; }

        /// <summary>
        /// The token security algorithm.
        /// </summary>
        /// <value>The token security algorithm.</value>        
        public TokenSecurityAlgorithms? TokenSecurityAlgorithm { get; set; }

        /// <summary>
        /// The signing token credential.
        /// </summary>
        /// <value>The signing token credential.</value>
        [JsonIgnore]
        internal ITokenCredential SigningTokenCredential { get; set; }

        /// <summary>
        /// The validating token credential.
        /// </summary>
        /// <value>The validating token credential.</value>
        [JsonIgnore]
        internal ITokenCredential ValidatingTokenCredential { get; set; }

        /// <summary>
        /// Token certificate algorithm.
        /// </summary>
        /// <value>The token certificate algorithm.</value>
        public string TokenCertificateAlgorithm { get; internal set; }

        /// <summary>
        /// The certificate algorithm.
        /// </summary>
        /// <value>The certificate algorithm.</value>
        public string CertificateAlgorithm { get; internal set; }

        #endregion

        #region Conditional Serializations

        /// <summary>
        /// Shoulds the serialize token security algorithm.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ShouldSerializeTokenSecurityAlgorithm()
        {
            return TokenSecurityAlgorithm.HasValue;
        }

        /// <summary>
        /// Shoulds the serialize token certificate algorithm.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ShouldSerializeTokenCertificateAlgorithm()
        {
            return !string.IsNullOrEmpty(TokenCertificateAlgorithm);
        }

        /// <summary>
        /// Shoulds the serialize certificate algorithm.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ShouldSerializeCertificateAlgorithm()
        {
            return !string.IsNullOrEmpty(CertificateAlgorithm);
        }

        #endregion

        #region Internal Class

        /// <inheritdoc />
        /// <summary>
        /// Class TokenCredential.
        /// </summary>
        /// <seealso cref="T:Dot.Net.Web.Api.Sdk.Models.Tokens.Interfaces.ITokenCredential" />
        internal class TokenCredential : ITokenCredential
        {
            /// <inheritdoc />
            /// <summary>
            /// Gets or sets the security key.
            /// </summary>
            /// <value>The security key.</value>
            public SecurityKey SecurityKey { get; set; }

            /// <inheritdoc />
            /// <summary>
            /// Gets or sets the signing credentials.
            /// </summary>
            /// <value>The signing credentials.</value>
            public SigningCredentials SigningCredentials { get; set; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtTokenModel"/> class.
        /// </summary>
        /// <param name="tokenName">Name of the token.</param>
        /// <param name="definition">The definition.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException">ValidatingCertificate</exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        [SuppressMessage("ReSharper", "NotResolvedInText")]
        public JwtTokenModel(string tokenName, TokenDefinitionElement definition)
        {
            TokenName = tokenName.ToUpper();

            var validIssuer = Uri.TryCreate(definition.Issuer, UriKind.Absolute, out var uriResult)
                              && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!validIssuer)
            {
                throw new ArgumentException(definition.Issuer);
            }

            TokenIssuer = definition.Issuer;

            if (!Regex.IsMatch(definition.IntendedAudience, URN_PATTERN, RegexOptions.IgnorePatternWhitespace))
            {
                throw new ArgumentException(definition.IntendedAudience);
            }

            TokenIntendedAudience = definition.IntendedAudience;

            TokenExpirationInMinutes = definition.ExpirationInMinute;
            IsTokenBase64Encoded = definition.IsBase64Encoded;
            OneTimeUse = definition.OneTimeUse;

            var passPhrase = definition.Signature.PassPhrase;

            if (!string.IsNullOrEmpty(passPhrase))
            {
                TokenSecurityType = TokenSecurityTypes.PassPhrase;

                SetPassPhraseSignature(passPhrase);

                return;
            }

            TokenSecurityAlgorithm = null;

            if (string.IsNullOrEmpty(definition.Signature.ValidatingCertificate) && string.IsNullOrEmpty(definition.Signature.SigningCertificate))
            {
                throw new ArgumentNullException("ValidatingCertificate");
            }

            byte[] validatingContent = null;
            byte[] signingContent = null;

            if (!string.IsNullOrEmpty(definition.Signature.ValidatingCertificate))
            {
                var validationCertificateFile = SearchCertificate(definition.Signature.ValidatingCertificate);

                if (string.IsNullOrEmpty(validationCertificateFile))
                {
                    throw new FileNotFoundException(definition.Signature.ValidatingCertificate);
                }

                validatingContent = File.ReadAllBytes(validationCertificateFile);
            }

            if (!string.IsNullOrEmpty(definition.Signature.SigningCertificate))
            {
                var signingCertificateFile = SearchCertificate(definition.Signature.SigningCertificate);

                signingContent = File.ReadAllBytes(signingCertificateFile);

                if (string.IsNullOrEmpty(signingCertificateFile))
                {
                    throw new FileNotFoundException(definition.Signature.SigningCertificate);
                }
            }

            TokenSecurityType = TokenSecurityTypes.Certificate;

            SetCertificateSignature(validatingContent, signingContent, definition.Signature.SigningCertificatePassword);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Searches the certificate.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        private static string SearchCertificate(string name)
        {
            var rootPath = HttpContext.Current.Server.MapPath(@"\");
            var certificate = Directory.GetFiles(rootPath, name, SearchOption.AllDirectories).FirstOrDefault();

            return certificate;
        }

        /// <summary>
        /// Sets the pass phrase signature.
        /// </summary>
        /// <param name="passPhrase">The pass phrase.</param>
        private void SetPassPhraseSignature(string passPhrase)
        {
            TokenCertificateAlgorithm = null;
            CertificateAlgorithm = null;

            passPhrase = Convert.ToBase64String(Encoding.UTF8.GetBytes(passPhrase));

            SigningTokenCredential = new TokenCredential
            {
                SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(passPhrase)),
            };

            if (!TokenSecurityAlgorithm.HasValue)
            {
                TokenSecurityAlgorithm = TokenSecurityAlgorithms.HmacSha256;
            }

            SigningTokenCredential.SigningCredentials = new SigningCredentials(SigningTokenCredential.SecurityKey,
                    TokenSecurityAlgorithm.Equals(TokenSecurityAlgorithms.HmacSha256)
                        ? SecurityAlgorithms.HmacSha256
                        : TokenSecurityAlgorithm.Equals(TokenSecurityAlgorithms.HmacSha384)
                            ? SecurityAlgorithms.HmacSha384
                            : TokenSecurityAlgorithm.Equals(TokenSecurityAlgorithms.HmacSha512)
                                ? SecurityAlgorithms.HmacSha512
                                : SecurityAlgorithms.HmacSha256);

            ValidatingTokenCredential = SigningTokenCredential;
        }

        /// <summary>
        /// Gets the certificate algorithm.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        /// <returns>System.String.</returns>
        private static string GetCertificateAlgorithm(X509Certificate2 certificate)
        {
            return certificate.SignatureAlgorithm.FriendlyName.ToUpper();
        }

        /// <summary>
        /// Sets the certificate signature.
        /// </summary>
        /// <param name="validatingContent">Content of the validating.</param>
        /// <param name="signingContent">Content of the signing.</param>
        /// <param name="signingCertificatePassword">The signing certificate password.</param>
        private void SetCertificateSignature(byte[] validatingContent, byte[] signingContent, string signingCertificatePassword = null)
        {
            TokenCertificateAlgorithm = SecurityAlgorithms.RsaSha256;

            if (validatingContent != null && validatingContent.Length > 0)
            {
                var validatingCertificate = new X509Certificate2();

                validatingCertificate.Import(validatingContent);

                ValidatingTokenCredential = new TokenCredential
                {
                    SecurityKey = new X509SecurityKey(validatingCertificate)
                };

                ValidatingTokenCredential.SigningCredentials = new SigningCredentials(ValidatingTokenCredential.SecurityKey, SecurityAlgorithms.RsaSha256);

                CertificateAlgorithm = GetCertificateAlgorithm(validatingCertificate);
            }

            if (signingContent == null || signingContent.Length <= 0)
            {
                return;
            }

            var signingCertificate = new X509Certificate2();

            if (signingCertificatePassword == null)
            {
                signingCertificate.Import(signingContent);
            }
            else
            {
                signingCertificate.Import(signingContent, signingCertificatePassword, X509KeyStorageFlags.Exportable);
            }

            SigningTokenCredential = new TokenCredential
            {
                SecurityKey = new X509SecurityKey(signingCertificate)
            };

            CertificateAlgorithm = GetCertificateAlgorithm(signingCertificate);

            SigningTokenCredential.SigningCredentials = new SigningCredentials(SigningTokenCredential.SecurityKey, SecurityAlgorithms.RsaSha256);
        }

        #endregion
    }
}
