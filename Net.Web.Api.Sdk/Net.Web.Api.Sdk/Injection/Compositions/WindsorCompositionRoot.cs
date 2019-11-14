using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Castle.Windsor;

namespace Net.Web.Api.Sdk.Injection.Compositions
{
    /// <inheritdoc />
    /// <summary>
    /// Class WindsorCompositionRoot.
    /// </summary>
    /// <seealso cref="T:System.Web.Http.Dispatcher.IHttpControllerActivator" />
    public class WindsorCompositionRoot : IHttpControllerActivator
    {
        #region Private Properties

        /// <summary>
        /// The container
        /// </summary>
        private readonly IWindsorContainer _container;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorCompositionRoot"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public WindsorCompositionRoot(IWindsorContainer container)
        {
            _container = container;
        }

        #endregion

        #region IHttpControllerActivator Implementations 

        /// <inheritdoc />
        /// <summary>
        /// Creates an <see cref="T:System.Web.Http.Controllers.IHttpController" /> object.
        /// </summary>
        /// <param name="request">The message request.</param>
        /// <param name="controllerDescriptor">The HTTP controller descriptor.</param>
        /// <param name="controllerType">The type of the controller.</param>
        /// <returns>An <see cref="T:System.Web.Http.Controllers.IHttpController" /> object.</returns>
        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = (IHttpController)_container.Resolve(controllerType);

            request.RegisterForDispose(new Release(() => _container.Release(controller)));

            return controller;
        }

        #endregion

        #region Private Classes

        /// <inheritdoc />
        /// <summary>
        /// Class Release. This class cannot be inherited.
        /// </summary>
        /// <seealso cref="T:System.IDisposable" />
        private sealed class Release : IDisposable
        {
            #region Private Properties

            /// <summary>
            /// The release
            /// </summary>
            private readonly Action _release;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Release"/> class.
            /// </summary>
            /// <param name="release">The release.</param>
            public Release(Action release)
            {
                _release = release;
            }

            #endregion

            #region IDisposable Implementations

            /// <inheritdoc />
            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _release();
            }

            #endregion
        }

        #endregion
    }
}