using System;
using Microsoft.Practices.Unity;

namespace Kingo.Messaging
{    
    /// <summary>
    /// Provides an implementation of the <see cref="MessageHandlerFactory" /> by using a I<see cref="UnityContainer" />.
    /// </summary>
    public sealed class UnityContainerFactory : MessageHandlerFactory
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
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityContainerFactory" /> class.
        /// </summary>
        /// <param name="container">
        /// Container that will be used by this factory. If <c>null</c>, this factory will create a new container.
        /// </param>
        public UnityContainerFactory(IUnityContainer container = null)
        {            
            Container = container ?? new UnityContainer();            
        }        

        /// <summary>
        /// Returns the container that is used by this factory to register and resolve message handlers.
        /// </summary>
        public IUnityContainer Container
        {
            get;
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
                Container.RegisterType(to, new TransientLifetimeManager());
            }
            else
            {
                Container.RegisterType(from, to);    
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
            Container.RegisterType(to, new PerUnitOfWorkLifetimeManager());

            if (from != null)
            {
                Container.RegisterType(from, to);
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
            Container.RegisterType(to, new ContainerControlledLifetimeManager());

            if (from != null)
            {
                Container.RegisterType(from, to);
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
                Container.RegisterInstance(to.GetType(), to);
            }
            else
            {
                Container.RegisterInstance(from, to);    
            }
            return this;      
        }

        /// <inheritdoc />
        protected override object Resolve(Type type) =>
            Container.Resolve(type);

        #endregion
    }
}
