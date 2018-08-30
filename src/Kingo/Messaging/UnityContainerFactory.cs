using System;
using Microsoft.Practices.Unity;

namespace Kingo.Messaging
{        
    internal sealed class UnityContainerFactory : MessageHandlerFactory
    {
        #region [====== PerUnitOfWorkLifetimeManager ======]
        
        internal sealed class PerUnitOfWorkLifetimeManager : LifetimeManager
        {
            private readonly string _key;

            public PerUnitOfWorkLifetimeManager()
            {
                _key = Guid.NewGuid().ToString("N");
            }

            /// <inheritdoc />
            public override object GetValue() =>
                Cache[_key];

            /// <inheritdoc />
            public override void SetValue(object newValue) =>
                Cache[_key] = newValue;

            /// <inheritdoc />
            public override void RemoveValue() =>
                Cache.Remove(_key);

            private static IUnitOfWorkCache Cache =>
                MicroProcessorContext.Current.UnitOfWork.Cache;            
        }

        #endregion

        private readonly IUnityContainer _container;
               
        public UnityContainerFactory()
        {            
            _container = new UnityContainer();            
        }                

        #region [====== Type Registration ======]

        /// <inheritdoc />
        protected override MessageHandlerFactory RegisterPerResolve(Type from, Type to)
        {
            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }
            if (from == null)
            {
                _container.RegisterType(to, new TransientLifetimeManager());
            }
            else
            {
                _container.RegisterType(from, to);    
            }
            return this;        
        }       

        /// <inheritdoc />
        protected override MessageHandlerFactory RegisterPerUnitOfWork(Type from, Type to)
        {
            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }            
            _container.RegisterType(to, new PerUnitOfWorkLifetimeManager());

            if (from != null)
            {
                _container.RegisterType(from, to);
            }
            return this;
        }        

        /// <inheritdoc />
        protected override MessageHandlerFactory RegisterPerProcessor(Type from,Type to)
        {
            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }
            _container.RegisterType(to, new ContainerControlledLifetimeManager());

            if (from != null)
            {
                _container.RegisterType(from, to);
            }
            return this;
        }

        /// <inheritdoc />
        public override MessageHandlerFactory RegisterInstance(Type from, object to)
        {
            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }
            if (from == null)
            {
                _container.RegisterInstance(to.GetType(), to);
            }
            else
            {
                _container.RegisterInstance(from, to);    
            }
            return this;      
        }
        
        protected internal override object Resolve(Type type) =>
            _container.Resolve(type);

        #endregion
    }
}
