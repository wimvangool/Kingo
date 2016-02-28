using System;
using Microsoft.Practices.Unity;

namespace Kingo.Messaging
{    
    internal sealed class UnityFactory : MessageHandlerFactory
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
        
        public UnityFactory()
        {            
            _container = new UnityContainer();
        }        

        #region [====== Type Registration ======]

        /// <inheritdoc />
        public override void RegisterWithPerResolveLifetime(Type concreteType, Type abstractType = null)
        {
            if (concreteType == null)
            {
                throw new ArgumentNullException(nameof(concreteType));
            }
            if (abstractType == null)
            {
                _container.RegisterType(concreteType, new TransientLifetimeManager());
            }
            else
            {
                _container.RegisterType(abstractType, concreteType);    
            }            
        }       

        /// <inheritdoc />
        public override void RegisterWithPerUnitOfWorkLifetime(Type concreteType, Type abstractType = null)
        {
            if (concreteType == null)
            {
                throw new ArgumentNullException(nameof(concreteType));
            }            
            _container.RegisterType(concreteType, new PerUnitOfWorkLifetimeManager());

            if (abstractType != null)
            {
                _container.RegisterType(abstractType, concreteType);
            }
        }        

        /// <inheritdoc />
        public override void RegisterSingleton(Type concreteType, Type abstractType = null)
        {
            if (concreteType == null)
            {
                throw new ArgumentNullException(nameof(concreteType));
            }
            _container.RegisterType(concreteType, new ContainerControlledLifetimeManager());

            if (abstractType != null)
            {
                _container.RegisterType(abstractType, concreteType);
            }
        }

        /// <inheritdoc />
        public override void RegisterSingleton(object concreteType, Type abstractType = null)
        {
            if (concreteType == null)
            {
                throw new ArgumentNullException(nameof(concreteType));
            }
            if (abstractType == null)
            {
                _container.RegisterInstance(concreteType.GetType(), concreteType);
            }
            else
            {
                _container.RegisterInstance(abstractType, concreteType);    
            }            
        }

        /// <inheritdoc />
        protected internal override object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        #endregion
    }
}
