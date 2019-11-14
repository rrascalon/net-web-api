using MultipartDataMediaFormatter.Infrastructure;
using Net.Web.Api.Sdk.Attributes.Validations;
using System.ComponentModel.DataAnnotations;

namespace Net.Web.Api.Sdk.Models.Services.Common
{
    /// <summary>
    /// Class UploadRequest.
    /// </summary>
    public class UploadRequest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the file information.
        /// </summary>
        /// <value>The file information.</value>
        [Required]
        [UploadFile]
        public HttpFile FileInformation { get; set; }

        #endregion
    }
}
