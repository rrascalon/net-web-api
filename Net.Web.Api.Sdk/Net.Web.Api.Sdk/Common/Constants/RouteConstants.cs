namespace Net.Web.Api.Sdk.Common.Constants
{
    /// <summary>
    /// Class RouteConstants.
    /// </summary>
    public static class RouteConstants
    {
        #region Public Constants

        /// <summary>
        /// The route prefix version
        /// </summary>
        public const string ROUTE_PREFIX_VERSION = "api/v{api-version:" + API_VERSION_FIELD + "}";

        #endregion

        #region Internal Constants

        /// <summary>
        /// The API version field
        /// </summary>
        internal const string API_VERSION_FIELD = "apiVersion";

        #endregion
    }
}
