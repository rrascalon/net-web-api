using System;
using System.Configuration;

namespace Net.Web.Api.Sdk.Configurations.Token
{
    /// <inheritdoc />
    /// <summary>
    /// Class TokenElementCollection.
    /// </summary>
    /// <seealso cref="T:System.Configuration.ConfigurationElementCollection" />
    public class TokenElementCollection : ConfigurationElementCollection
    {
        #region Private Constants

        /// <summary>
        /// The collection element name
        /// </summary>
        private const string COLLECTION_ELEMENT_NAME = "token";

        #endregion

        #region ConfigurationElementCollection Overrides

        /// <inheritdoc />
        /// <summary>
        /// Gets the name used to identify this collection of elements in the configuration file when overridden in a derived class.
        /// </summary>
        /// <value>The name of the element.</value>
        protected override string ElementName => COLLECTION_ELEMENT_NAME;

        /// <inheritdoc />
        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </summary>
        /// <returns>A newly created <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new TokenElement();
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to return the key for.</param>
        /// <returns>An <see cref="T:System.Object" /> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TokenElement)element).Name;
        }

        /// <inheritdoc />
        /// <summary>
        /// Indicates whether the specified <see cref="T:System.Configuration.ConfigurationElement" /> exists in the <see cref="T:System.Configuration.ConfigurationElementCollection" />.
        /// </summary>
        /// <param name="elementName">The name of the element to verify.</param>
        /// <returns>true if the element exists in the collection; otherwise, false. The default is false.</returns>
        protected override bool IsElementName(string elementName)
        {
            return !string.IsNullOrEmpty(elementName) && COLLECTION_ELEMENT_NAME.Equals(elementName, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection" />.
        /// </summary>
        /// <value>The type of the collection.</value>
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

        /// <summary>
        /// Gets the <see cref="TokenElement"/> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>TokenElement.</returns>
        public new TokenElement this[string key] => BaseGet(key) as TokenElement;

        /// <summary>
        /// Gets the <see cref="TokenElement"/> with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>TokenElement.</returns>
        public TokenElement this[int id] => BaseGet(id) as TokenElement;

        #endregion
    }
}
