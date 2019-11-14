using System;
using System.Web.Http;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Net.Web.Api.Sdk.Injection.Installers
{
    /// <inheritdoc />
    /// <summary>
    /// Class ControllerInstaller.
    /// </summary>
    /// <seealso cref="T:Castle.MicroKernel.Registration.IWindsorInstaller" />
    public class ControllerInstaller : IWindsorInstaller
    {
        #region IWindsorInstaller Implementations

        /// <inheritdoc />
        /// <summary>
        /// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer" />.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(AppDomain.CurrentDomain.RelativeSearchPath))
                .BasedOn<ApiController>()
                .LifestylePerWebRequest());
        }

        #endregion
    }
}