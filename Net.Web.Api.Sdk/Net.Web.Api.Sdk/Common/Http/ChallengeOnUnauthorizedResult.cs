using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Net.Web.Api.Sdk.Common.Http
{
    /// <summary>
    /// Class AddChallengeOnUnauthorizedResult.
    /// Implements the <see cref="IHttpActionResult" />
    /// </summary>
    /// <seealso cref="IHttpActionResult" />
    public class ChallengeOnUnauthorizedResult : IHttpActionResult
    {
        #region Properties

        /// <summary>
        /// Gets the challenge.
        /// </summary>
        /// <value>The challenge.</value>
        public AuthenticationHeaderValue Challenge { get; }

        /// <summary>
        /// Gets the inner result.
        /// </summary>
        /// <value>The inner result.</value>
        public IHttpActionResult InnerResult { get; }

        #endregion

        #region IHttpActionResult Implementations

        /// <inheritdoc />
        /// <summary>
        /// execute as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="T:System.Net.Http.HttpResponseMessage" />.</returns>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await InnerResult.ExecuteAsync(cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            if (response.Headers.WwwAuthenticate.All(h => h.Scheme != Challenge.Scheme))
            {
                response.Headers.WwwAuthenticate.Add(Challenge);
            }

            return response;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeOnUnauthorizedResult"/> class.
        /// </summary>
        /// <param name="challenge">The challenge.</param>
        /// <param name="innerResult">The inner result.</param>
        public ChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IHttpActionResult innerResult)
        {
            Challenge = challenge;
            InnerResult = innerResult;
        }

        #endregion
    }
}
