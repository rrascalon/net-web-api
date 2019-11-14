using System;

namespace Net.Web.Api.Sdk.Documentation.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Class SwaggerOperationOrderAttribute.
    /// </summary>
    /// <seealso cref="T:System.Attribute" />
    public class SwaggerOperationOrderAttribute : Attribute
    {
        #region Public Enums

        /// <summary>
        /// Enum OperationFrom
        /// </summary>
        public enum OperationFrom
        {
            /// <summary>
            /// The SDK
            /// </summary>
            Sdk,

            /// <summary>
            /// The application
            /// </summary>
            Application
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        public OperationFrom From { get; set; }

        /// <summary>
        /// Gets the operation tags.
        /// </summary>
        /// <value>The operation tags.</value>
        public string[] OperationTags { get; set; }

        #endregion
    }
}
