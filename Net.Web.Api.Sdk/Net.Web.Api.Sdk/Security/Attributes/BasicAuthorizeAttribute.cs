using Net.Web.Api.Sdk.Common.Http;
using Net.Web.Api.Sdk.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Formatting;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Net.Web.Api.Sdk.Security.Attributes
{
    /// <summary>
    /// Class BasicAuthorizeAttribute.
    /// Implements the <see cref="Attribute" />
    /// Implements the <see cref="IAuthenticationFilter" />
    /// </summary>
    /// <seealso cref="Attribute" />
    /// <seealso cref="IAuthenticationFilter" />
    public abstract class BasicAuthorizeAttribute : Attribute, IAuthenticationFilter
    {
        #region Internal Constants

        /// <summary>
        /// The authorization basic
        /// </summary>
        internal const string AUTHORIZATION_BASIC = "Basic";

        /// <summary>
        /// The realm
        /// </summary>
        internal const string REALM = "realm";

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether [enable challenge].
        /// </summary>
        /// <value><c>true</c> if [enable challenge]; otherwise, <c>false</c>.</value>
        public bool EnableChallenge { get; set; }

        /// <summary>
        /// Gets or sets the realm.
        /// </summary>
        /// <value>The realm.</value>
        public string Realm { get; set; }

        #endregion

        #region Private Properties

        /// <summary>
        /// The default formatter
        /// </summary>
        private static readonly JsonMediaTypeFormatter _defaultFormatter = new JsonMediaTypeFormatter
        {
            SerializerSettings =
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }
        };

        #endregion

        #region Abscrtact Methods

        /// <summary>
        /// Authenticates the asynchronous.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="authenticationResult">The authentication result.</param>
        /// <returns>Task&lt;IPrincipal&gt;.</returns>
        protected abstract Task<IPrincipal> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken, IList<string> authenticationResult);

        #endregion

        #region Attribute Overrides

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets a value indicating whether more than one instance of the indicated attribute can be specified for a single program element.
        /// </summary>
        /// <value><c>true</c> if [allow multiple]; otherwise, <c>false</c>.</value>
        public virtual bool AllowMultiple => false;

        #endregion

        #region IAuthenticationFilter Implementations

        /// <summary>
        /// authenticate as an asynchronous operation.
        /// </summary>
        /// <param name="context">The authentication context.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A Task that will perform authentication.</returns>
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null)
            {
                context.ErrorResult = new ResponseActionResult(request, HttpStatusCode.Forbidden);

                return;
            }

            if (!AUTHORIZATION_BASIC.Equals(authorization.Scheme))
            {
                context.ErrorResult = new ResponseActionResult(request, HttpStatusCode.Forbidden);

                return;
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new ResponseActionResult(request, HttpStatusCode.Unauthorized);

                return;
            }

            var userNameAndPasword = ExtractUserNameAndPassword(authorization.Parameter);

            if (userNameAndPasword == null)
            {
                context.ErrorResult = new ResponseActionResult(request, HttpStatusCode.Unauthorized);

                return;
            }

            var authenticationResult = new List<string>();
            var userName = userNameAndPasword.Item1;
            var password = userNameAndPasword.Item2;
            var principal = await AuthenticateAsync(userName, password, cancellationToken, authenticationResult);

            if (principal == null)
            {
                context.ErrorResult = new ResponseActionResult(request, HttpStatusCode.Unauthorized, authenticationResult);
            }
            else
            {
                context.Principal = principal;
            }
        }

        /// <summary>
        /// Challenges the asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            if (EnableChallenge)
            {
                Challenge(context);
            }

            return Task.FromResult(0);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Extracts the user name and password.
        /// </summary>
        /// <param name="authorizationParameter">The authorization parameter.</param>
        /// <returns>TupleModel&lt;System.String, System.String&gt;.</returns>
        private static Tuple<string, string> ExtractUserNameAndPassword(string authorizationParameter)
        {
            byte[] credentialBytes;

            try
            {
                credentialBytes = Convert.FromBase64String(authorizationParameter);
            }
            catch (FormatException)
            {
                return null;
            }

            var encoding = Encoding.ASCII;

            encoding = (Encoding)encoding.Clone();

            encoding.DecoderFallback = DecoderFallback.ExceptionFallback;

            string decodedCredentials;

            try
            {
                decodedCredentials = encoding.GetString(credentialBytes);
            }
            catch (DecoderFallbackException)
            {
                return null;
            }

            if (string.IsNullOrEmpty(decodedCredentials))
            {
                return null;
            }

            var colonIndex = decodedCredentials.IndexOf(':');

            if (colonIndex == -1)
            {
                return null;
            }

            var userName = decodedCredentials.Substring(0, colonIndex);
            var password = decodedCredentials.Substring(colonIndex + 1);

            return new Tuple<string, string>(userName, password);
        }

        /// <summary>
        /// Challenges the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            var parameter = string.IsNullOrEmpty(Realm) ? null : $@"{REALM}="" + Realm + @""";

            context.ChallengeWith(AUTHORIZATION_BASIC, parameter);
        }

        #endregion
    }
}
