using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Castle.Windsor;
using Net.Web.Api.Sdk.Injection.Scopes;

namespace Net.Web.Api.Sdk.Injection.Resolvers
{
    /// <inheritdoc />
    /// <summary>
    /// Class WindsorDependencyResolver.
    /// </summary>
    /// <seealso cref="T:System.Web.Http.Dependencies.IDependencyResolver" />
    public class WindsorDependencyResolver : IDependencyResolver
    {
        #region Private Properties

        private readonly IWindsorContainer _container;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorDependencyResolver"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public WindsorDependencyResolver(IWindsorContainer container)
        {
            _container = container;
        }

        #endregion

        #region IDependencyResolver Implementations

        /// <inheritdoc />
        /// <summary>
        /// Starts a resolution scope.
        /// </summary>
        /// <returns>The dependency scope.</returns>
        public IDependencyScope BeginScope()
        {
            return new WindsorDependencyScope(_container);
        }

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
            return !_container.Kernel.HasComponent(serviceType) ? new object[0] : _container.ResolveAll(serviceType).Cast<object>();
        }

        /// <inheritdoc />
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _container.Dispose();
        }

        #endregion
    }
}