using Net.Web.Api.Sdk.Interfaces.Information;
using Net.Web.Api.Sdk.Interfaces.Token;
using Net.Web.Api.Sdk.Properties;
using NuGet;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Net.Web.Api.Sdk.Implementations.Information
{
    /// <summary>
    /// Class InformationService.
    /// Implements the <see cref="IInformationService" />
    /// </summary>
    /// <seealso cref="IInformationService" />
    public sealed class InformationService : IInformationService
    {
        #region Private Constants

        /// <summary>
        /// The package information file name
        /// </summary>
        private const string PACKAGE_INFORMATION_FILE_NAME = "packages.info";

        /// <summary>
        /// The nuget information file name
        /// </summary>
        private const string NUGET_INFORMATION_FILE_NAME = "packages.config";

        #endregion

        #region Services

        /// <summary>
        /// The token service
        /// </summary>
        private readonly IJwtTokenService _tokenService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationService"/> class.
        /// </summary>
        /// <param name="tokenService">The token service.</param>
        /// <exception cref="ArgumentNullException">tokenService</exception>
        public InformationService(IJwtTokenService tokenService)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        #endregion

        #region IInformationService Implementations

        /// <summary>
        /// Gets the SDK informations.
        /// </summary>
        /// <returns>dynamic.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public dynamic GetSdkInformations()
        {           
            var assembly = Assembly.GetExecutingAssembly();
            var sourceResource = $"{assembly.GetName().Name}.{NUGET_INFORMATION_FILE_NAME}";
            var rootPath = HttpContext.Current.Server.MapPath(@"\");
            var nugetPackageConfigFileName = Path.Combine(rootPath, PACKAGE_INFORMATION_FILE_NAME);

            string content = null;

            using (var stream = assembly.GetManifestResourceStream(sourceResource))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        content = reader.ReadToEnd();
                    }
                }
            }

            if (!string.IsNullOrEmpty(content))
            {               
                File.WriteAllText(nugetPackageConfigFileName, content);
            }

            dynamic result = new ExpandoObject();

            result.library = GetAssemblyInformations();

            var tokens = _tokenService.Tokens.Select(c=>c.Value).ToList().OrderBy(c=>c.TokenName);

            result.availableTokens = tokens;

            if(File.Exists(nugetPackageConfigFileName))
            {
                var packageConfiguration = new PackageReferenceFile(nugetPackageConfigFileName);
                var allPacakges = packageConfiguration.GetPackageReferences();
                var nugetPackages = new List<dynamic>();

                foreach (var package in allPacakges)
                {
                    dynamic onePackage = new ExpandoObject();

                    onePackage.name = package.Id;
                    onePackage.version = package.Version.ToString();
                    onePackage.framework = package.TargetFramework.Version.ToString();

                    nugetPackages.Add(onePackage);
                }

                result.packages = nugetPackages;
            }

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the assembly informations.
        /// </summary>
        /// <returns>dynamic.</returns>
        private dynamic GetAssemblyInformations()
        {
            var libraryAssembly = Assembly.GetAssembly(GetType());
            var description = libraryAssembly.GetCustomAttribute(typeof(AssemblyDescriptionAttribute)) as AssemblyDescriptionAttribute;
            var copyright = libraryAssembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute;
            var title = libraryAssembly.GetCustomAttribute(typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute;
            var version = libraryAssembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute)) as AssemblyFileVersionAttribute;
            var author = libraryAssembly.GetCustomAttribute(typeof(AssemblyCompanyAttribute)) as AssemblyCompanyAttribute;

            dynamic result = new ExpandoObject();

            result.title = title.Title;
            result.description = description.Description;
            result.version = version.Version;
            result.copyright = copyright.Copyright;
            result.author = author.Company;
            result.license = Resources.License;

            return result;
        }

        #endregion
    }
}
