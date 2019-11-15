using System;
using System.Globalization;
using System.IO;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using Castle.MicroKernel.ModelBuilder.Inspectors;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Microsoft.IdentityModel.Logging;
using Microsoft.Web.Http;
using Microsoft.Web.Http.Description;
using Microsoft.Web.Http.Routing;
using MultipartDataMediaFormatter;
using MultipartDataMediaFormatter.Infrastructure;
using Net.Web.Api.Sdk.Common.Constants;
using Net.Web.Api.Sdk.Common.Validations;
using Net.Web.Api.Sdk.Documentation.Filters;
using Net.Web.Api.Sdk.Injection.Compositions;
using Net.Web.Api.Sdk.Injection.Containers;
using Net.Web.Api.Sdk.Injection.Installers;
using Net.Web.Api.Sdk.Injection.Resolvers;
using Net.Web.Api.Sdk.Interfaces.Token;
using Net.Web.Api.Sdk.Security.Handlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Application;

namespace Net.Web.Api.Sdk.Initialization
{
    /// <summary>
    /// Class HttpConfigurationExtensions.
    /// </summary>
    public static class HttpConfigurationExtensions
    {
        #region Private Constants

        /// <summary>
        /// The kit swagger configuration
        /// </summary>
        private const string KIT_SWAGGER_CONFIGURATION = "SwaggerConfigurationSdk.json";

        /// <summary>
        /// The kit default token configuration
        /// </summary>
        private const string KIT_DEFAULT_TOKEN_CONFIGURATION = "token-sdk.config";

        /// <summary>
        /// The kit documentation
        /// </summary>
        private const string KIT_DOCUMENTATION = "doc-api-sdk.xml";

        /// <summary>
        /// The kit index
        /// </summary>
        private const string KIT_INDEX = "swagger.html";

        #endregion

        #region Public Extensions

        /// <summary>
        /// Registers the web API.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="formatter">The formatter.</param>
        public static void RegisterWebApi(this HttpConfiguration configuration, JsonMediaTypeFormatter formatter = null)
        {
            configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            configuration.SetupInjection();
            configuration.SetupApi(formatter);
            configuration.EnsureInitialized();
        }

        /// <summary>
        /// Uns the register web API.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public static void UnRegisterWebApi(this HttpConfiguration configuration)
        {
            InjectionContainer.Instance.DisposeContainer();
        }

        #endregion

        #region Private Extensions

        /// <summary>
        /// Setups the injection.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        private static void SetupInjection(this HttpConfiguration configuration)
        {
            var container = new WindsorContainer();

            container.Install(new ControllerInstaller());
            container.Install(new ServiceInstaller());

            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));

            var dependencyResolver = new WindsorDependencyResolver(container);

            configuration.DependencyResolver = dependencyResolver;

            configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(container));            

            InjectionContainer.Instance.SetContainer(container);
        }

        /// <summary>
        /// Setups the API.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="formatter">The formatter.</param>
        private static void SetupApi(this HttpConfiguration configuration, JsonMediaTypeFormatter formatter = null)
        {
            formatter = configuration.SetupFormatters(formatter);

            var apiExplorer = configuration.SetupVersioning();

            configuration.SetupSecurity();

            configuration.SetupDocumentation(apiExplorer);

            configuration.Filters.Add(new ParameterValidationActionFilterAttribute(formatter));

            var service = InjectionContainer.Instance.GetService<IJwtTokenService>();

            service?.CleanupTokenDatabase();
        }

        /// <summary>
        /// Setups the formatters.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="formatter">The formatter.</param>
        /// <returns>JsonMediaTypeFormatter.</returns>
        private static JsonMediaTypeFormatter SetupFormatters(this HttpConfiguration configuration, JsonMediaTypeFormatter formatter = null)
        {
            configuration.Formatters.Clear();

            var fmt = formatter ?? new JsonMediaTypeFormatter
            {
                SerializerSettings =
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            };

            configuration.Formatters.Add(fmt);

            var multipartSettings = new MultipartFormatterSettings
            {
                CultureInfo = CultureInfo.CurrentCulture,
                SerializeByteArrayAsHttpFile = true,
                ValidateNonNullableMissedProperty = true
            };

            configuration.Formatters.Add(new FormMultipartEncodedMediaTypeFormatter(multipartSettings));

            return fmt;
        }

        /// <summary>
        /// Setups the versioning.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>VersionedApiExplorer.</returns>
        private static VersionedApiExplorer SetupVersioning(this HttpConfiguration configuration)
        {
            var constraintResolver = new DefaultInlineConstraintResolver();

            constraintResolver.ConstraintMap.Add(RouteConstants.API_VERSION_FIELD, typeof(ApiVersionRouteConstraint));

            configuration.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            configuration.MapHttpAttributeRoutes(constraintResolver);

            var apiExplorer = configuration.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            return apiExplorer;
        }

        /// <summary>
        /// Setups the security.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        private static void SetupSecurity(this HttpConfiguration configuration)
        {
            IdentityModelEventSource.ShowPII = true;

            configuration.MessageHandlers.Add(new JwtTokenHandler());

            var assembly = Assembly.GetExecutingAssembly();

            ExtractTextResource(assembly, EmbeddedResourceConstants.SECURITY_ASSEMBLY_NAMESPACE, KIT_DEFAULT_TOKEN_CONFIGURATION, KIT_DEFAULT_TOKEN_CONFIGURATION);
        }

        /// <summary>
        /// Setups the documentation.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="apiExplorer">The API explorer.</param>
        private static void SetupDocumentation(this HttpConfiguration configuration, VersionedApiExplorer apiExplorer)
        {
            const string SWAGGER_PATH_PREFIX = "{" + RouteConstants.API_VERSION_FIELD + "}/swagger";

            var swaggerConfiguration = configuration.EnableSwagger(SWAGGER_PATH_PREFIX, swaggerDocConfig =>
            {
                swaggerDocConfig.MultipleApiVersions(
                    (apiDescription, version) => apiDescription.GetGroupName() == version,
                    info =>
                    {
                        if (apiExplorer.ApiDescriptions == null)
                        {
                            return;
                        }

                        foreach (var group in apiExplorer.ApiDescriptions)
                        {
                            var description = string.Empty;

                            if (group.IsDeprecated)
                            {
                                description += "This Service version has been deprecated.";
                            }

                            info.Version(group.Name, $"Version {group.ApiVersion}").Description(description);
                        }
                    }
                );

                swaggerDocConfig.Schemes(new[] { "http", "https" });
                swaggerDocConfig.PrettyPrint();
                swaggerDocConfig.DescribeAllEnumsAsStrings();
                swaggerDocConfig.IgnoreObsoleteActions();
                swaggerDocConfig.IgnoreObsoleteProperties();

                swaggerDocConfig.OperationFilter<SwaggerConsumesFilter>();
                swaggerDocConfig.OperationFilter<SwaggerProducesFilter>();
                swaggerDocConfig.OperationFilter<SwaggerUploadOperationFilter>();
                swaggerDocConfig.OperationFilter<SwaggerSecurityTypeAttributeFilter>();

                swaggerDocConfig.DocumentFilter<SwaggerMethodOrderingFilter>();
                swaggerDocConfig.DocumentFilter<SwaggerOperationOrderingFilter>();

                var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}";
                var files = Directory.GetFiles(basePath, "doc-api-*.xml");

                foreach (var file in files)
                {
                    swaggerDocConfig.IncludeXmlComments(file);
                }
            });

            var assembly = Assembly.GetExecutingAssembly();

            swaggerConfiguration.EnableSwaggerUi(c =>
            {               
                c.CustomAsset("index", assembly, $"{EmbeddedResourceConstants.RESOURCE_ASSEMBLY_NAMESPACE}.index.html");
                c.InjectJavaScript(assembly, $"{EmbeddedResourceConstants.RESOURCE_ASSEMBLY_NAMESPACE}.swagger-ui-override.js");
                c.InjectStylesheet(assembly, $"{EmbeddedResourceConstants.RESOURCE_ASSEMBLY_NAMESPACE}.swagger-ui-override.css");
                c.EnableDiscoveryUrlSelector();
                c.DisableValidator();
            });

            ExtractTextResource(assembly, EmbeddedResourceConstants.RESOURCE_ASSEMBLY_NAMESPACE, KIT_SWAGGER_CONFIGURATION, KIT_SWAGGER_CONFIGURATION);
            ExtractTextResource(assembly, EmbeddedResourceConstants.RESOURCE_ASSEMBLY_NAMESPACE, KIT_DOCUMENTATION, KIT_DOCUMENTATION);
            ExtractTextResource(assembly, EmbeddedResourceConstants.RESOURCE_ASSEMBLY_NAMESPACE, KIT_INDEX, "swagger.html");
        }

        /// <summary>
        /// Extracts the text resource.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="source">The source.</param>
        /// <param name="destin">The destin.</param>
        private static void ExtractTextResource(Assembly assembly, string nameSpace, string source, string destin)
        {
            var sourceResource = $"{nameSpace}.{source}";
            var rootPath = HttpContext.Current.Server.MapPath(@"\");

            var content = string.Empty;

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

            var fileName = $@"{rootPath}{destin}";

            File.WriteAllText(fileName, content);
        }

        #endregion
    }
}
