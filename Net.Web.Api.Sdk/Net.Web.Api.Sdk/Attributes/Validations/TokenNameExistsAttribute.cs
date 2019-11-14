using Net.Web.Api.Sdk.Injection.Containers;
using Net.Web.Api.Sdk.Interfaces.Token;
using Net.Web.Api.Sdk.Properties;
using System;
using System.ComponentModel.DataAnnotations;

namespace Net.Web.Api.Sdk.Attributes.Validations
{
    /// <summary>
    /// Class TokenNameExistsAttribute.
    /// Implements the <see cref="ValidationAttribute" />
    /// </summary>
    /// <seealso cref="ValidationAttribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public class TokenNameExistsAttribute : ValidationAttribute
    {
        #region ValidationAttribute Overrides

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var service = InjectionContainer.Instance.GetService<IJwtTokenService>();
            var tokenName = value != null ? value.ToString().Trim().ToUpper() : string.Empty;
            var exists = service.Tokens.ContainsKey(tokenName);

            if(exists)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(string.Format(Resources.TokenNameNotFound, tokenName));
        }

        #endregion
    }
}
