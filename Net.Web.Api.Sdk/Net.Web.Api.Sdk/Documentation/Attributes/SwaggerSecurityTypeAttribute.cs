using System;

namespace Net.Web.Api.Sdk.Documentation.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Class SwaggerSecurityTypeAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SwaggerSecurityTypeAttribute : Attribute
    {
        #region Public Properties

        /// <summary>
        /// Gets the type of the security.
        /// </summary>
        /// <value>The type of the security.</value>
        public string SecurityType { get; }

        #endregion

        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Web.Api.Toolkit.Attributes.Swagger.SwaggerSecurityTypeAttribute" /> class.
        /// </summary>
        /// <param name="securityType">Type of the security.</param>
        public SwaggerSecurityTypeAttribute(string securityType)
        {
            SecurityType = securityType;
        }

        #endregion
    }
}
