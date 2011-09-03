namespace Manssiere.Core
{
    using System;
    using System.Collections.Generic;
    using Castle.Windsor;
    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// An adapter allowing an <see cref="IWindsorContainer"/> to plug into Caliburn via <see cref="IServiceLocator"/> and <see cref="IConfigurator"/>.
    /// </summary>
    public class WindsorAdapter : ServiceLocatorImplBase, IContainerAccessor
    {
        private readonly IWindsorContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorAdapter"/> class.
        /// </summary>
        /// <param name="container">The _container.</param>
        public WindsorAdapter(IWindsorContainer container)
        {
            _container = container;

            _container.Kernel.AddComponentInstance<IServiceLocator>(typeof (IServiceLocator), this);
            _container.Kernel.AddComponentInstance<IContainerAccessor>(typeof (IContainerAccessor), this);
            _container.Kernel.AddComponentInstance<IWindsorContainer>(typeof (IWindsorContainer), _container);
        }

        #region IContainerAccessor Members

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public IWindsorContainer Container
        {
            get { return _container; }
        }

        #endregion

        /// <summary>
        /// When implemented by inheriting classes, this method will do the actual work of resolving
        /// the requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="key">Name of registered service you want. May be null.</param>
        /// <returns>
        /// The requested service instance.
        /// </returns>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (key != null)
            {
                return serviceType != null ? _container.Resolve(key, serviceType) : _container.Resolve(key);
            }

            return _container.Resolve(serviceType);
        }

        /// <summary>
        ///When implemented by inheriting classes, this method will do the actual work of
        ///resolving all the requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of service requested.</param>
        /// <returns>
        /// Sequence of service instance objects.
        /// </returns>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return (object[]) _container.ResolveAll(serviceType);
        }
    }
}