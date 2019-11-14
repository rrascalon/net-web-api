using System;
using System.Collections.Generic;

namespace Net.Web.Api.Sdk.Documentation.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Class SwaggerConsumesAttribute.
    /// </summary>
    public class SwaggerConsumesAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets the content types.
        /// </summary>
        /// <value>The content types.</value>
        public IEnumerable<string> ContentTypes { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerConsumesAttribute"/> class.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        public SwaggerConsumesAttribute(string contentType)
        {
            ContentTypes = new List<string> { contentType };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerConsumesAttribute"/> class.
        /// </summary>
        /// <param name="contentTypes">The content types.</param>
        public SwaggerConsumesAttribute(params string[] contentTypes)
        {
            ContentTypes = contentTypes;
        }

        #endregion
    }
}
