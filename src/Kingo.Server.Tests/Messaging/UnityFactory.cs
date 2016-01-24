using System;
using Microsoft.Practices.Unity;

namespace Kingo.Messaging
{
    public sealed class UnityFactory : MessageHandlerFactory
    {
        #region [====== PerUnitOfWorkLifetimeManager ======]

        /// <summary>
        /// A LifetimeManager for Unity that registers it's dependencies at the current <see cref="UnitOfWorkContext" />.
        /// </summary>
        public sealed class PerUnitOfWorkLifetimeManager : LifetimeManager
        {
            private IDependencyCacheEntry<object> _entry;

            /// <inheritdoc />
            public override object GetValue()
            {
                object value;

                if (_entry != null && _entry.TryGetValue(out value))
                {
                    return value;
                }
                return null;
            }

            /// <inheritdoc />
            public override void RemoveValue()
            {
                if (_entry == null)
                {
                    return;
                }
                _entry.Dispose();
                _entry = null;
            }

            /// <inheritdoc />
            public override void SetValue(object newValue)
            {
                IDependencyCache cache;

                if (TryGetDependencyCache(out cache))
                {
                    _entry = cache.Add(newValue, HandleValueInvalidated);
                }
                else
                {
                    HandleValueInvalidated(newValue);
                }
            }

            private static bool TryGetDependencyCache(out IDependencyCache cache)
            {
                var context = UnitOfWorkContext.Current;
                if (context == null)
                {
                    cache = null;
                    return false;
                }
                cache = context.Cache;
                return true;
            }

            private static void HandleValueInvalidated(object value)
            {
                var disposable = value as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        #endregion

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
        protected internal override void RegisterWithPerResolveLifetime(Type type)
        {
            _container.RegisterType(type, new TransientLifetimeManager());
        }

        /// <inheritdoc />
        protected internal override void RegisterWithPerResolveLifetime(Type concreteType, Type abstractType)
        {
            _container.RegisterType(abstractType, concreteType);
        }

        /// <inheritdoc />
        protected internal override void RegisterWithPerUnitOfWorkLifetime(Type type)
        {
            _container.RegisterType(type, new PerUnitOfWorkLifetimeManager());
        }

        /// <inheritdoc />
        protected internal override void RegisterWithPerUnitOfWorkLifetime(Type concreteType, Type abstractType)
        {
            _container.RegisterType(concreteType, new PerUnitOfWorkLifetimeManager());
            _container.RegisterType(abstractType, concreteType);
        }

        /// <inheritdoc />
        protected internal override void RegisterSingleton(Type type)
        {
            _container.RegisterType(type, new ContainerControlledLifetimeManager());
        }

        /// <inheritdoc />
        protected internal override void RegisterSingleton(Type concreteType, Type abstractType)
        {
            _container.RegisterType(concreteType, new ContainerControlledLifetimeManager());
            _container.RegisterType(abstractType, concreteType);
        }

        /// <inheritdoc />
        protected internal override void RegisterSingleton(object concreteType, Type abstractType)
        {
            _container.RegisterInstance(abstractType, concreteType);
        }

        /// <inheritdoc />
        protected internal override object CreateMessageHandler(Type type)
        {
            return _container.Resolve(type);
        }

        #endregion
    }
}
