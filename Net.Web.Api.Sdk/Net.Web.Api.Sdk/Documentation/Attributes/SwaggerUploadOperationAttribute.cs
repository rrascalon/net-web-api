using System;

namespace Net.Web.Api.Sdk.Documentation.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Class SwaggerUploadOperationAttribute.
    /// </summary>
    /// <seealso cref="T:System.Attribute" />
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerUploadOperationAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        /// <value>The type of the parameter.</value>
        public Type ParameterType { get; }

        #endregion

        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Net.Web.Api.Sdk.Documentation.Attributes.SwaggerUploadOperationAttribute" /> class.
        /// </summary>
        /// <param name="parameterType">Type of the parameter.</param>
        public SwaggerUploadOperationAttribute(Type parameterType)
        {
            ParameterType = parameterType;
        }

        #endregion
    }
}
