using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;

namespace Net.Web.Api.Sdk.Injection.Scopes
{
    /// <inheritdoc />
    /// <summary>
    /// Class WindsorDependencyScope.
    /// </summary>
    /// <seealso cref="T:System.Web.Http.Dependencies.IDependencyScope" />
    public class WindsorDependencyScope : IDependencyScope
    {
        #region Private Properties

        /// <summary>
        /// The container
        /// </summary>
        private readonly IWindsorContainer _container;

        /// <summary>
        /// The scope
        /// </summary>
        private readonly IDisposable _scope;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorDependencyScope"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public WindsorDependencyScope(IWindsorContainer container)
        {
            _container = container;
            _scope = container.BeginScope();
        }

        #endregion

        #region IDependencyScope Implementations

        /// <inheritdoc />
        /// <summary>
        /// Retrieves a service from the scope.
        /// </summary>
        /// <param name="serviceType">The service to be retrieved.</param>
        /// <returns>The retrieved service.</returns>
        public object GetService(Type serviceType)
        {
            return _container.Kernel.HasComponent(serviceType) ? _container.Resolve(serviceType) : null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Retrieves a collection of services from the scope.
        /// </summary>
        /// <param name="serviceType">The collection of services to be retrieved.</param>
        /// <returns>The retrieved collection of services.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.ResolveAll(serviceType).Cast<object>();
        }

        /// <inheritdoc />
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _scope.Dispose();
        }

        #endregion
    }
}
