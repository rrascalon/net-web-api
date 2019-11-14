using Net.Web.Api.Sdk.Models.Token;
using Net.Web.Api.Sdk.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Net.Web.Api.Sdk.Attributes.Validations
{
    /// <summary>
    /// Class TokenPayloadValidAttribute.
    /// Implements the <see cref="ValidationAttribute" />
    /// </summary>
    /// <seealso cref="ValidationAttribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public class TokenPayloadValidAttribute : ValidationAttribute
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
            var name = string.IsNullOrEmpty(validationContext.DisplayName)
                ? validationContext.MemberName
                : validationContext.DisplayName;

            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value != null && value.GetType() != typeof(List<KeyValuePair<string, string>>))
            {
                return new ValidationResult(string.Format(GetDefaultRequiredMessage(), name));
            }

            var collection = value as List<KeyValuePair<string, string>>;

            if (collection.Count == 0)
            {
                return ValidationResult.Success;
            }

            var keys = collection.Select(record => record.Key).ToList();

            if (keys.Where(string.IsNullOrEmpty).ToList().Count > 0)
            {
                return new ValidationResult(Resources.TokenPayloadEmptyKeyNoAllowed);
            }

            var values = collection.Select(record => record.Value).ToList();

            if (values.Where(string.IsNullOrEmpty).ToList().Count > 0)
            {
                return new ValidationResult(Resources.TokenPayloadEmptyValuesAreNotAllowed);
            }

            var duplicated = (from rec in keys
                              group rec by rec
                              into grouped
                              where grouped.Count() > 1
                              select grouped.Key).ToList();

            if (duplicated.Count > 0)
            {
                return new ValidationResult(string.Format(Resources.TokenPayloadDuplicatedKeys, string.Join(", ", duplicated)));
            }

            var reserved = Enum.GetNames(typeof(TokenInternalClaimNames)).ToList();
            var intersection = reserved.Intersect(keys).ToList();

            if (intersection.Count <= 0)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(string.Format(Resources.TokenPayloadReservedKeys, string.Join(", ", intersection)));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the default required message.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string GetDefaultRequiredMessage()
        {
            var message = string.Empty;
            var assembly = Assembly.GetAssembly(typeof(RequiredAttribute));

            foreach (var type in assembly.GetTypes())
            {
                if (!type.Name.Equals("DataAnnotationsResources"))
                {
                    continue;
                }

                var property = type.GetProperty("RequiredAttribute_ValidationError", BindingFlags.NonPublic | BindingFlags.Static);

                if (property == null)
                {
                    continue;
                }

                message = (string)property.GetValue(null);

                break;
            }

            return !string.IsNullOrEmpty(message) ? message : Resources.FieldRequiredText;
        }

        #endregion
    }
}
