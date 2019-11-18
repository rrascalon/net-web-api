using Net.Web.Api.Sdk.Interfaces.File;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Net.Web.Api.Sdk.Implementations.File
{
    /// <summary>
    /// Class FileService.
    /// Implements the <see cref="IFileService" />
    /// </summary>
    /// <seealso cref="IFileService" />
    public class FileService : IFileService
    {
        #region Constants

        /// <summary>
        /// The upload directory name
        /// </summary>
        private const string UPLOAD_DIRECTORY_NAME = "Upload";

        #endregion

        #region ICommonService Implementations

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Uri.</returns>
        public Uri UploadFile(byte[] content, string fileName)
        {
            var rootPath = HttpContext.Current.Server.MapPath(@"\");
            var destinationDirectory = Path.Combine(rootPath, UPLOAD_DIRECTORY_NAME);

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            var destinationFile = GetUniqueFileName(Path.Combine(destinationDirectory, fileName));

            System.IO.File.WriteAllBytes(destinationFile, content);

            var url = HttpContext.Current.Request.Url.AbsoluteUri;

            url = url.Replace(HttpContext.Current.Request.Url.AbsolutePath, string.Empty);

            return new Uri($"{url}/{UPLOAD_DIRECTORY_NAME}/{Path.GetFileName(destinationFile)}");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the name of the unique file.
        /// </summary>
        /// <param name="fullFileName">Full name of the file.</param>
        /// <returns>System.String.</returns>
        private static string GetUniqueFileName(string fullFileName)
        {
            if (!System.IO.File.Exists(fullFileName))
            {
                return fullFileName;
            }

            var folder = Path.GetDirectoryName(fullFileName);

            if (folder == null)
            {
                return fullFileName;
            }

            var filename = Path.GetFileNameWithoutExtension(fullFileName);
            var extension = Path.GetExtension(fullFileName);
            var number = 1;
            var regEx = Regex.Match(fullFileName, @"(.+) \((\d+)\)\.\w+");

            if (regEx.Success)
            {
                filename = regEx.Groups[1].Value;
                number = int.Parse(regEx.Groups[2].Value);
            }

            do
            {
                number++;

                fullFileName = Path.Combine(folder, $"{filename} ({number}){extension}");
            }
            while (System.IO.File.Exists(fullFileName));

            return fullFileName;
        }

        #endregion
    }
}
