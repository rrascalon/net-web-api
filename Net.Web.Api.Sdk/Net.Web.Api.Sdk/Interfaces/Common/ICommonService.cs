using Net.Web.Api.Sdk.Injection.Attributes;
using System;

namespace Net.Web.Api.Sdk.Interfaces.Common
{
    /// <summary>
    /// Interface ICommonService
    /// </summary>
    [InjectInterfaceService]
    public interface ICommonService
    {
        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Uri.</returns>
        Uri UploadFile(byte[] content, string fileName);
    }
}
