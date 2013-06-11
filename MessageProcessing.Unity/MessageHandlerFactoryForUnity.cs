using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Implements a <see cref="MessageHandlerFactory"/> by using a <see cref="IUnityContainer" />.
    /// </summary>
    public class MessageHandlerFactoryForUnity : MessageHandlerFactory, IUnityContainer
    {
        private readonly IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerFactoryForUnity" /> class.
        /// </summary>
        /// <param name="container">The container to use.</param>
        public MessageHandlerFactoryForUnity(IUnityContainer container = null)
        {
            if (container == null)
            {
                container = new UnityContainer();
            }
            _container = container;            
        }        

        #region [====== UnityContainer Members ======]

        /// <inheritdoc />
        public IUnityContainer Parent
        {
            get { return _container.Parent; }
        }

        /// <inheritdoc />
        public IEnumerable<ContainerRegistration> Registrations
        {
            get { return _container.Registrations; }
        }

        /// <inheritdoc />
        public IUnityContainer AddExtension(UnityContainerExtension extension)
        {
            return _container.AddExtension(extension);
        }

        /// <inheritdoc />
        public object BuildUp(Type t, object existing, string name, params ResolverOverride[] resolverOverrides)
        {
            return _container.BuildUp(t, existing, name, resolverOverrides);
        }

        /// <inheritdoc />
        public object Configure(Type configurationInterface)
        {
            return _container.Configure(configurationInterface);
        }

        /// <inheritdoc />
        IUnityContainer IUnityContainer.CreateChildContainer()
        {
            return _container.CreateChildContainer();
        }

        /// <inheritdoc />
        IUnityContainer IUnityContainer.RegisterInstance(Type t, string name, object instance, LifetimeManager lifetime)
        {
            return RegisterInstance(t, name, instance, lifetime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="name"></param>
        /// <param name="instance"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public MessageHandlerFactoryForUnity RegisterInstance(Type t, string name, object instance, LifetimeManager lifetime)
        {
            return new MessageHandlerFactoryForUnity(_container.RegisterInstance(t, name, instance, lifetime));
        }

        /// <inheritdoc />
        IUnityContainer IUnityContainer.RegisterType(Type from, Type to, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            return RegisterType(from, to, name, lifetimeManager, injectionMembers);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="name"></param>
        /// <param name="lifetimeManager"></param>
        /// <param name="injectionMembers"></param>
        /// <returns></returns>
        public MessageHandlerFactoryForUnity RegisterType(Type from, Type to, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            return new MessageHandlerFactoryForUnity(_container.RegisterType(from, to, name, lifetimeManager, injectionMembers));
        }

        /// <inheritdoc />
        IUnityContainer IUnityContainer.RemoveAllExtensions()
        {
            return _container.RemoveAllExtensions();
        }

        /// <inheritdoc />
        public object Resolve(Type t, string name, params ResolverOverride[] resolverOverrides)
        {
            return _container.Resolve(t, name, resolverOverrides);
        }

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type t, params ResolverOverride[] resolverOverrides)
        {
            return _container.ResolveAll(t, resolverOverrides);
        }

        /// <inheritdoc />
        public void Teardown(object o)
        {
            _container.Teardown(o);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _container.Dispose();
        }

        #endregion

        #region [====== Members of MessageHandlerFactory ======]

        protected override void RegisterWithPerResolveLifetime(Type type)
        {
            _container.RegisterType(type, new TransientLifetimeManager());
        }

        protected override void RegisterWithPerUnitOfWorkLifetime(Type type)
        {
            _container.RegisterType(type, new PerUnitOfWorkLifetimeManager());
        }

        protected override void RegisterSingle(Type type)
        {
            _container.RegisterType(type, new ContainerControlledLifetimeManager());
        }

        protected override object CreateMessageHandler(Type type)
        {
            return _container.Resolve(type);
        }

        #endregion
    }
}
