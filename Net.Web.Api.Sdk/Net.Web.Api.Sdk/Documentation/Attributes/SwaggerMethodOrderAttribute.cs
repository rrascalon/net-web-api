using System;

namespace Net.Web.Api.Sdk.Documentation.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Class SwaggerMethodOrderAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerMethodOrderAttribute : Attribute
    {
        #region Public Properties

        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order { get; }

        #endregion

        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Web.Api.Toolkit.Attributes.Swagger.SwaggerMethodOrderAttribute" /> class.
        /// </summary>
        /// <param name="order">The order.</param>
        public SwaggerMethodOrderAttribute(int order)
        {
            Order = order;
        }

        #endregion
    }
}
