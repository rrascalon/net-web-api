namespace Net.Web.Api.Sdk.Extensions
{
    /// <summary>
    /// Class StringExtensions.
    /// </summary>
    public static class StringExtensions
    {
        #region Public Methods

        /// <summary>
        /// To the secured encoded64 padding.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>System.String.</returns>
        public static string ToSecuredEncoded64Padding(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return str.EndsWith("=") ? str.Replace("=", "EQUAL") : str;
        }

        /// <summary>
        /// Froms the secured encoded64 padding.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>System.String.</returns>
        public static string FromSecuredEncoded64Padding(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return str.EndsWith("EQUAL") ? str.Replace("EQUAL", "=") : str;
        }

        #endregion
    }
}
