using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Net.Web.Api.Sdk.Common.Http
{
    /// <summary>
    /// Class ResponseActionResult.
    /// Implements the <see cref="IHttpActionResult" />
    /// </summary>
    /// <seealso cref="IHttpActionResult" />
    public class ResponseActionResult : IHttpActionResult
    {
        #region Private Properties

        /// <summary>
        /// The request
        /// </summary>
        private readonly HttpRequestMessage _request;


        /// <summary>
        /// The status code
        /// </summary>
        private readonly HttpStatusCode _statusCode;

        /// <summary>
        /// The content
        /// </summary>
        private readonly object _content;

        /// <summary>
        /// The formatter
        /// </summary>
        private readonly JsonMediaTypeFormatter _formatter;

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

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseActionResult" /> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="content">The content.</param>
        /// <param name="formatter">The formatter.</param>
        public ResponseActionResult(HttpRequestMessage request, HttpStatusCode statusCode, object content = null, JsonMediaTypeFormatter formatter = null)
        {
            _request = request;
            _statusCode = statusCode;
            _content = content;
            _formatter = formatter ?? _defaultFormatter;
        }

        #endregion

        #region IHttpActionResult Implementations

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage" /> asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that, when completed, contains the <see cref="T:System.Net.Http.HttpResponseMessage" />.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = _request.CreateResponse(_statusCode, _content, _formatter);

            return Task.FromResult(response);
        }

        #endregion
    }
}
