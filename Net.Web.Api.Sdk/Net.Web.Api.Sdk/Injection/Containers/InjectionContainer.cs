using System;
using Castle.Windsor;

namespace Net.Web.Api.Sdk.Injection.Containers
{
    /// <summary>
    /// Class InjectionContainer. This class cannot be inherited.
    /// </summary>
    public sealed class InjectionContainer
    {
        #region Singleton

        /// <summary>
        /// The lazy
        /// </summary>
        private static readonly Lazy<InjectionContainer> _lazy = new Lazy<InjectionContainer>(() => new InjectionContainer());

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static InjectionContainer Instance => _lazy.Value;

        #endregion

        #region Private Properties

        /// <summary>
        /// The container
        /// </summary>
        private IWindsorContainer _container;

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="InjectionContainer"/> class from being created.
        /// </summary>
        private InjectionContainer() { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the container.
        /// </summary>
        /// <param name="container">The container.</param>
        public void SetContainer(IWindsorContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        public T GetService<T>()
        {
            return _container.Resolve<T>();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Disposes the container.
        /// </summary>
        internal void DisposeContainer()
        {
            _container?.Dispose();
        }

        #endregion

    }
}
