namespace Net.Web.Api.Sdk.Models.Services.Token
{
    /// <summary>
    /// Class CreateTokenResult.
    /// </summary>
    public class CreateTokenResult
    {
        #region Public Properties

        /// <summary>
        /// The JWT token created.
        /// </summary>
        /// <value>The access token.</value>
        public string AccessToken { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTokenResult"/> class.
        /// </summary>
        /// <param name="token">The token.</param>
        public CreateTokenResult(string token)
        {
            AccessToken = token;
        }

        #endregion

    }
}
