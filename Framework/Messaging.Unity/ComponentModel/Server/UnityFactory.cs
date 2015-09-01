using System;
using Microsoft.Practices.Unity;

namespace ServiceComponents.ComponentModel.Server
{
    /// <summary>
    /// Implements a <see cref="MessageHandlerFactory"/> by using a <see cref="Microsoft.Practices.Unity.IUnityContainer" />.
    /// </summary>
    public class UnityFactory : MessageHandlerFactory
    {
        private readonly IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityFactory" /> class.
        /// </summary>        
        /// <param name="container">The container to use.</param>
        public UnityFactory(IUnityContainer container = null)
        {
            if (container == null)
            {
                container = new UnityContainer();
            }
            _container = container;             
        }        
        
        /// <summary>
        /// Returns the container that is used to store all <see cref="IMessageHandler{T}">MessageHandlers</see>
        /// and other dependencies.
        /// </summary>
        public IUnityContainer Container
        {
            get { return _container; }
        }

        #region [====== Members of MessageHandlerFactory ======]

        /// <inheritdoc />
        protected override void RegisterWithPerResolveLifetime(Type type)
        {
            _container.RegisterType(type, new TransientLifetimeManager());
        }

        /// <inheritdoc />
        protected override void RegisterWithPerResolveLifetime(Type concreteType, Type abstractType)
        {
            _container.RegisterType(abstractType, concreteType);
        }

        /// <inheritdoc />
        protected override void RegisterWithPerUnitOfWorkLifetime(Type type)
        {
            _container.RegisterType(type, PerUnitOfWorkLifetime());
        }

        /// <inheritdoc />
        protected override void RegisterWithPerUnitOfWorkLifetime(Type concreteType, Type abstractType)
        {
            _container.RegisterType(concreteType, PerUnitOfWorkLifetime());
            _container.RegisterType(abstractType, concreteType);
        }        

        /// <inheritdoc />
        protected override void RegisterSingleton(Type type)
        {
            _container.RegisterType(type, new ContainerControlledLifetimeManager());
        }

        /// <inheritdoc />
        protected override void RegisterSingleton(Type concreteType, Type abstractType)
        {
            _container.RegisterType(concreteType, new ContainerControlledLifetimeManager());
            _container.RegisterType(abstractType, concreteType);
        }

        /// <inheritdoc />
        protected override object CreateMessageHandler(Type type)
        {
            return _container.Resolve(type);
        }

        private LifetimeManager PerUnitOfWorkLifetime()
        {
            return new CacheBasedLifetimeManager(UnitOfWorkCache);
        }        

        #endregion
    }
}
