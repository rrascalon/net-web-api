using System.Configuration;

namespace Net.Web.Api.Sdk.Configurations.Token
{
    /// <inheritdoc />
    /// <summary>
    /// Class TokenElement.
    /// </summary>
    /// <seealso cref="T:System.Configuration.ConfigurationElement" />
    public class TokenElement : ConfigurationElement
    {
        #region Private Constants

        /// <summary>
        /// The token name
        /// </summary>
        private const string TOKEN_NAME = "name";

        /// <summary>
        /// The token definition
        /// </summary>
        private const string TOKEN_DEFINITION = "definition";

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty(TOKEN_NAME, IsRequired = true, IsKey = true)]
        public string Name => (string)this[TOKEN_NAME];

        /// <summary>
        /// Gets the definition.
        /// </summary>
        /// <value>The definition.</value>
        [ConfigurationProperty(TOKEN_DEFINITION, IsRequired = true, IsKey = false)]
        public TokenDefinitionElement Definition => (TokenDefinitionElement)this[TOKEN_DEFINITION];

        #endregion
    }
}
