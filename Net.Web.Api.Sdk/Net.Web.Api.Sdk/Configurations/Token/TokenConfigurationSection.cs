using System.Configuration;

namespace Net.Web.Api.Sdk.Configurations.Token
{
    /// <inheritdoc />
    /// <summary>
    /// Class TokenConfigurationSection.
    /// </summary>
    /// <seealso cref="T:System.Configuration.ConfigurationSection" />
    public class TokenConfigurationSection : ConfigurationSection
    {
        #region Private Constants

        /// <summary>
        /// The token section name
        /// </summary>
        internal const string SECTION_NAME = "tokenSection";

        /// <summary>
        /// The collection name
        /// </summary>
        private const string COLLECTION_NAME = "tokens";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        [ConfigurationProperty(COLLECTION_NAME, IsDefaultCollection = true, IsKey = false, IsRequired = true)]
        public TokenElementCollection Members
        {
            get => base[COLLECTION_NAME] as TokenElementCollection;

            set => base[COLLECTION_NAME] = value;
        }

        #endregion
    }
}
