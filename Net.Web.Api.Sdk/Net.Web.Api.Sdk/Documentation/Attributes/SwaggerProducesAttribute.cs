using System;
using System.Collections.Generic;

namespace Net.Web.Api.Sdk.Documentation.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Class SwaggerProducesAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerProducesAttribute : Attribute
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
        /// Initializes a new instance of the <see cref="SwaggerProducesAttribute"/> class.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        public SwaggerProducesAttribute(string contentType)
        {
            ContentTypes = new List<string> { contentType };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerProducesAttribute"/> class.
        /// </summary>
        /// <param name="contentTypes">The content types.</param>
        public SwaggerProducesAttribute(params string[] contentTypes)
        {
            ContentTypes = contentTypes;
        }

        #endregion
    }
}
