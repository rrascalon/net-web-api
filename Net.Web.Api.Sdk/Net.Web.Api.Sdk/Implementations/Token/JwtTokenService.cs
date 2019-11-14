using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using LiteDB;
using Microsoft.IdentityModel.Tokens;
using Net.Web.Api.Sdk.Configurations.Token;
using Net.Web.Api.Sdk.Extensions;
using Net.Web.Api.Sdk.Interfaces.Token;
using Net.Web.Api.Sdk.Models.Token;

namespace Net.Web.Api.Sdk.Implementations.Token
{
    /// <inheritdoc />
    /// <summary>
    /// Class JwtTokenService.
    /// </summary>
    /// <seealso cref="T:Net.Web.Api.Sdk.Interfaces.Token.IJwtTokenService" />
    public class JwtTokenService : IJwtTokenService
    {
        #region Constants

        /// <summary>
        /// The root path
        /// </summary>
        private const string ROOT_PATH = @"\";

        /// <summary>
        /// The token config file pattern
        /// </summary>
        private const string TOKEN_CONFIG_FILE_PATTERN = "token*.config";

        /// <summary>
        /// The token data collection
        /// </summary>
        private const string TOKEN_DATA_COLLECTION = "tokens";

        #endregion

        #region Public Properties

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the tokens.
        /// </summary>
        /// <value>The tokens.</value>
        public Dictionary<string, JwtTokenModel> Tokens { get; private set; }

        #endregion

        #region Private Properties

        /// <summary>
        /// The token data base
        /// </summary>
        private string _tokenDataBase;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtTokenService"/> class.
        /// </summary>
        public JwtTokenService()
        {
            LoadAllTokens();
            SetupTokenDatabase();
        }

        #endregion

        #region ITokenService Implementations

        /// <inheritdoc />
        /// <summary>
        /// Creates the token.
        /// </summary>
        /// <param name="tokenName">Name of the token.</param>
        /// <param name="identityName">Name of the identity.</param>
        /// <param name="customClaims">The custom claims.</param>
        /// <returns>System.String.</returns>
        public virtual string CreateToken(string tokenName, string identityName, Dictionary<string, string> customClaims = null)
        {
            if (string.IsNullOrEmpty(tokenName))
            {
                throw new ArgumentNullException(nameof(tokenName));
            }

            tokenName = tokenName.ToUpper();

            var isTokenDefined = Tokens.ContainsKey(tokenName);

            if (!isTokenDefined)
            {
                throw new KeyNotFoundException(tokenName);
            }

            var definition = Tokens[tokenName];
            var now = DateTime.UtcNow;
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = definition.TokenIssuer,
                Audience = definition.TokenIntendedAudience,
                IssuedAt = now,
                Expires = now.AddMinutes(definition.TokenExpirationInMinutes),
                Subject = GetTokenSubject(definition, identityName, customClaims),
                SigningCredentials = definition.SigningTokenCredential.SigningCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);
            var generatedToken = tokenHandler.WriteToken(token);

            return FormatToken(generatedToken, definition);
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the token validation parameters.
        /// </summary>
        /// <param name="validateExipration">if set to <c>true</c> [validate exipration].</param>
        /// <param name="issuers">The issuers.</param>
        /// <param name="audiences">The audiences.</param>
        /// <returns>TokenValidationParameters.</returns>
        public virtual TokenValidationParameters GetTokenValidationParameters(
            bool validateExipration = false, 
            string issuers = null, 
            string audiences = null)
        {
            var vi = !string.IsNullOrEmpty(issuers);
            var va = !string.IsNullOrEmpty(audiences);

            var validationParameters = new TokenValidationParameters
            {
                ValidateActor = false,
                RequireSignedTokens = true,
                RequireExpirationTime = validateExipration,
                ValidateLifetime = validateExipration,
                ValidateIssuer = vi,
                ValidateAudience = va,
                ValidateIssuerSigningKey = false,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKeys = GetAllSecurityKeys()
            };

            if (vi)
            {
                validationParameters.ValidIssuers = issuers.Split(',').Select(p => p.Trim()).ToList();
            }

            if (va)
            {
                validationParameters.ValidAudiences = audiences.Split(',').Select(p => p.Trim()).ToList();
            }

            return validationParameters;
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the token payload.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public virtual Dictionary<string, string> GetTokenPayload(HttpActionContext context)
        {
            var identity = context.RequestContext.Principal.Identity;

            if (identity == null || !identity.IsAuthenticated)
            {
                return new Dictionary<string, string>();
            }

            context.GetToken(out var securityToken);

            return securityToken?.Claims?.ToClaimDictionary() ?? new Dictionary<string, string>();
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the identity payload.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public virtual Dictionary<string, string> GetIdentityPayload(HttpActionContext context)
        {
            var principal = context.RequestContext.Principal;
            var identity = principal?.Identity;

            if (identity == null || !identity.IsAuthenticated)
            {
                return new Dictionary<string, string>();
            }

            var claims = ((ClaimsIdentity) identity).Claims;

            var result = claims?.ToClaimDictionary(true) ?? new Dictionary<string, string>();
            
            return result;
        }

        /// <summary>
        /// Determines whether [is token revoked] [the specified token].
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="claims">The claims.</param>
        /// <returns><c>true</c> if [is token revoked] [the specified token]; otherwise, <c>false</c>.</returns>
        public virtual bool IsTokenRevoked(string token, List<Claim> claims)
        {
            var found = GetTokenState(token, claims, out var isRevoked, out _);

            if(!found)
            {
                return false;
            }

            return isRevoked;
        }

        /// <summary>
        /// Determines whether [is token used] [the specified token].
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="claims">The claims.</param>
        /// <returns><c>true</c> if [is token used] [the specified token]; otherwise, <c>false</c>.</returns>
        public virtual bool IsTokenUsed(string token, List<Claim> claims)
        {
            if(!claims.IsTokenOneTimeUse())
            {
                return false;
            }

            var found = GetTokenState(token, claims, out _, out var isUsed);

            if (!found)
            {
                return false;
            }

            return isUsed;
        }

        /// <summary>
        /// Revokes the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="claims">The claims.</param>
        public virtual bool RevokeToken(string token, List<Claim> claims)
        {
            var found = GetTokenState(token, claims, out _, out _);

            if(found)
            {
                return false;
            }

            return RevokeToken(token);
        }

        /// <summary>
        /// Marks the token as used.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="claims">The claims.</param>
        public virtual void MarkTokenAsUsed(string token, List<Claim> claims)
        {
            if (!claims.IsTokenOneTimeUse())
            {
                return;
            }

            var found = GetTokenState(token, claims, out _, out _);

            if (found)
            {
                return;
            }

            MarkTokenAsUsed(token);
        }

        /// <summary>
        /// Cleanups the token database.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public virtual int CleanupTokenDatabase()
        {
            var count = 0;

            using (var db = new LiteDatabase(_tokenDataBase))
            {
                var tokens = db.GetCollection<JwtTokenUsedOrRevoked>(TOKEN_DATA_COLLECTION);
                var now = DateTime.UtcNow;

                count = tokens.Delete(c => c.ExpirationDate.CompareTo(now) > 0);
            }

            return count;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtTokenService" /> class.
        /// </summary>
        /// <param name="token">The token.</param>
        private bool RevokeToken(string token)
        {
            using (var db = new LiteDatabase(_tokenDataBase))
            {
                var tokens = db.GetCollection<JwtTokenUsedOrRevoked>(TOKEN_DATA_COLLECTION);
                var found = tokens.Find(c => c.Token.Equals(token)).FirstOrDefault();

                if(found == null)
                {
                    var record = new JwtTokenUsedOrRevoked
                    {
                        Token = token,
                        IsRevoked = true,
                        ExpirationDate = token.GetExpirationDate(),
                        RevocationDate = DateTime.UtcNow
                    };

                    tokens.Insert(record);
                }
            }

            return true;
        }

        /// <summary>
        /// Marks the token as used.
        /// </summary>
        /// <param name="token">The token.</param>
        private void MarkTokenAsUsed(string token)
        {
            using (var db = new LiteDatabase(_tokenDataBase))
            {
                var tokens = db.GetCollection<JwtTokenUsedOrRevoked>(TOKEN_DATA_COLLECTION);
                var found = tokens.Find(c => c.Token.Equals(token)).FirstOrDefault();

                if (found == null)
                {
                    var record = new JwtTokenUsedOrRevoked
                    {
                        Token = token,
                        IsUsed = true,
                        ExpirationDate = token.GetExpirationDate(),
                        UsedDate = DateTime.UtcNow
                    };

                    tokens.Insert(record);
                }
            }
        }

        /// <summary>
        /// Gets the state of the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="claims">The claims.</param>
        /// <param name="isRevoked">if set to <c>true</c> [is revoked].</param>
        /// <param name="isUsed">if set to <c>true</c> [is used].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool GetTokenState(string token, List<Claim> claims, out bool isRevoked, out bool isUsed)
        {
            isRevoked = false;
            isUsed = false;

            if (string.IsNullOrEmpty(token) || claims == null || !claims.Any())
            {
                return false;
            }

            JwtTokenUsedOrRevoked found = null;

            using (var db = new LiteDatabase(_tokenDataBase))
            {
                var tokens = db.GetCollection<JwtTokenUsedOrRevoked>(TOKEN_DATA_COLLECTION);
                found = tokens.Find(c => c.Token.Equals(token)).FirstOrDefault();

                if (found != null)
                {
                    isUsed = found.IsUsed;
                    isRevoked = found.IsRevoked;
                }
            }

            return found != null;
        }

        /// <summary>
        /// Gets all security keys.
        /// </summary>
        /// <returns>IEnumerable&lt;SecurityKey&gt;.</returns>
        private IEnumerable<SecurityKey> GetAllSecurityKeys()
        {
            return (from td in Tokens
                where td.Value.ValidatingTokenCredential?.SecurityKey != null
                select td.Value.ValidatingTokenCredential.SecurityKey).ToList();
        }

        /// <summary>
        /// Gets the token subject.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="identityName">Name of the identity.</param>
        /// <param name="customClaims">The custom claims.</param>
        /// <returns>ClaimsIdentity.</returns>
        private static ClaimsIdentity GetTokenSubject(JwtTokenModel definition, 
            string identityName, 
            IReadOnlyDictionary<string, string> customClaims)
        {
            var identity = new ClaimsIdentity("Bearer");

            identity.AddClaim(new Claim(ClaimTypes.Name, identityName));
            identity.AddClaim(new Claim(ClaimTypes.PrimarySid, identityName));
            identity.AddClaim(new Claim(TokenInternalClaimNames.tn.ToString(), definition.TokenName));
            identity.AddClaim(new Claim(TokenInternalClaimNames.jti.ToString(), Guid.NewGuid().ToString()));
            identity.AddClaim(new Claim(TokenInternalClaimNames.exm.ToString(), definition.TokenExpirationInMinutes.ToString(CultureInfo.InvariantCulture)));
            identity.AddClaim(new Claim(TokenInternalClaimNames.otu.ToString(), definition.OneTimeUse.ToString().ToLower()));

            if (customClaims == null || customClaims.Count <= 0)
            {
                return identity;
            }

            var internals = Enum.GetValues(typeof(TokenInternalClaimNames)).Cast<TokenInternalClaimNames>().Select(v => v.ToString()).ToList();

            foreach (var claim in customClaims)
            {
                if (internals.Contains(claim.Key))
                {
                    continue;
                }

                identity.AddClaim(new Claim(claim.Key, claim.Value));
            }

            return identity;
        }

        /// <summary>
        /// Formats the token.
        /// </summary>
        /// <param name="generatedToken">The generated token.</param>
        /// <param name="definition">The definition.</param>
        /// <returns>System.String.</returns>
        private static string FormatToken(string generatedToken, JwtTokenModel definition)
        {
            return !definition.IsTokenBase64Encoded 
                ? generatedToken 
                : Convert.ToBase64String(Encoding.UTF8.GetBytes(generatedToken)).ToSecuredEncoded64Padding();
        }

        /// <summary>
        /// Loads the token list.
        /// </summary>
        /// <param name="tokenConfigurationSection">The token configuration section.</param>
        /// <returns>Dictionary&lt;System.String, ApiTokenModel&gt;.</returns>
        private static Dictionary<string, JwtTokenModel> LoadTokenList(TokenConfigurationSection tokenConfigurationSection)
        {
            var tokens = new Dictionary<string, JwtTokenModel>();

            for (var i = 0; i < tokenConfigurationSection.Members.Count; i++)
            {
                var definition = tokenConfigurationSection.Members[i].Definition;
                var tokenName = tokenConfigurationSection.Members[i].Name;

                if (definition?.Signature == null || tokens.ContainsKey(tokenName))
                {
                    continue;
                }

                tokens.Add(tokenName, new JwtTokenModel(tokenName, definition));
            }

            return tokens;
        }

        /// <summary>
        /// Loads all tokens.
        /// </summary>
        private void LoadAllTokens()
        {
            Tokens = new Dictionary<string, JwtTokenModel>();

            var rootPath = HttpContext.Current.Server.MapPath(ROOT_PATH);
            var configurationFileList = Directory.GetFiles(rootPath, TOKEN_CONFIG_FILE_PATTERN, SearchOption.AllDirectories);

            if (!configurationFileList.Any())
            {
                return;
            }            

            foreach (var configurationFile in configurationFileList)
            {
                var configMap = new ExeConfigurationFileMap { ExeConfigFilename = configurationFile };
                var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                var tokenSection = (TokenConfigurationSection) config.GetSection(TokenConfigurationSection.SECTION_NAME);
                var tokenList = LoadTokenList(tokenSection);

                foreach (var token in tokenList)
                {
                    if (!Tokens.ContainsKey(token.Key))
                    {
                        Tokens.Add(token.Key, token.Value);
                    }
                    else
                    {
                        Tokens[token.Key] = token.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Setups the token database.
        /// </summary>
        private void SetupTokenDatabase()
        {
            var rootPath = HttpContext.Current.Server.MapPath(ROOT_PATH);
            var dataBasePath = Path.Combine(rootPath, "db");

            if(!Directory.Exists(dataBasePath))
            {
                Directory.CreateDirectory(dataBasePath);
            }

            _tokenDataBase = Path.Combine(dataBasePath, "TokenDataBase.db");

            var mapper = BsonMapper.Global;

            mapper.Entity<JwtTokenUsedOrRevoked>().Id(c => c.Token);
        }

        #endregion
    }
}
