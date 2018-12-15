using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a builder of a very simple <see cref="IMessageHandlerFactory"/> that can only resolve
    /// message handlers with a default, public constructor.
    /// </summary>
    public sealed class SimpleMessageHandlerFactoryBuilder : MessageHandlerFactoryBuilder
    {
        #region [====== ServiceProvider ======]

        private sealed class ServiceProvider : IServiceProvider
        {
            private readonly Dictionary<Type, IServiceProvider> _registeredTypes;

            public ServiceProvider(Dictionary<Type, ServiceLifetime> registeredTypes)
            {
                _registeredTypes = new Dictionary<Type, IServiceProvider>();

                foreach (var registeredType in registeredTypes)
                {
                    switch (registeredType.Value)
                    {
                        case ServiceLifetime.Transient:
                            _registeredTypes[registeredType.Key] = new TransientServiceProvider();
                            break;

                        case ServiceLifetime.Scoped:
                            _registeredTypes[registeredType.Key] = new ScopedServiceProvider();
                            break;

                        case ServiceLifetime.Singleton:
                            _registeredTypes[registeredType.Key] = new SingletonServiceProvider();
                            break;
                    }

                }
            }

            public object GetService(Type serviceType)
            {
                if (_registeredTypes.TryGetValue(serviceType, out var serviceProvider))
                {
                    return serviceProvider.GetService(serviceType);
                }
                return null;
            }
        }

        private sealed class TransientServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType) =>
                Activator.CreateInstance(serviceType);
        }

        private sealed class ScopedServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType) =>
                throw new NotImplementedException();
        }

        private sealed class SingletonServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType) =>
                throw new NotImplementedException();
        }

        #endregion

        private readonly Dictionary<Type, ServiceLifetime> _registeredTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMessageHandlerFactoryBuilder" /> class.
        /// </summary>
        public SimpleMessageHandlerFactoryBuilder()
        {
            _registeredTypes = new Dictionary<Type, ServiceLifetime>();
        }

        protected override void RegisterTransient(Type type) =>
            _registeredTypes[type] = ServiceLifetime.Transient;

        protected override void RegisterScoped(Type type) =>
            _registeredTypes[type] = ServiceLifetime.Scoped;

        protected override void RegisterSingleton(Type type) =>
            _registeredTypes[type] = ServiceLifetime.Singleton;

        /// <inheritdoc />
        protected override IServiceProvider CreateServiceProvider() =>
            new ServiceProvider(_registeredTypes);
    }
}
