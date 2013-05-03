using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;

namespace YellowFlare.MessageProcessing
{
    public static class MessageHandlerFactoryForUnityExtensions
    {
        public static MessageHandlerFactoryForUnity RegisterType<T>(this MessageHandlerFactoryForUnity container, params InjectionMember[] injectionMembers)
        {            
            return container.RegisterType(null, typeof(T), null, null, injectionMembers);
        }

        public static MessageHandlerFactoryForUnity RegisterType<TFrom, TTo>(this MessageHandlerFactoryForUnity container, params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            return container.RegisterType(typeof(TFrom), typeof(TTo), null, null, injectionMembers);
        }

        public static MessageHandlerFactoryForUnity RegisterType<TFrom, TTo>(this MessageHandlerFactoryForUnity container, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom
        {           
            return container.RegisterType(typeof(TFrom), typeof(TTo), null, lifetimeManager, injectionMembers);
        }

        public static MessageHandlerFactoryForUnity RegisterType<TFrom, TTo>(this MessageHandlerFactoryForUnity container, string name, params InjectionMember[] injectionMembers) where TTo : TFrom
        {            
            return container.RegisterType(typeof(TFrom), typeof(TTo), name, null, injectionMembers);
        }

        public static MessageHandlerFactoryForUnity RegisterType<TFrom, TTo>(this MessageHandlerFactoryForUnity container, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers) where TTo : TFrom
        {            
            return container.RegisterType(typeof(TFrom), typeof(TTo), name, lifetimeManager, injectionMembers);
        }

        public static MessageHandlerFactoryForUnity RegisterType<T>(this MessageHandlerFactoryForUnity container, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {            
            return container.RegisterType(null, typeof(T), null, lifetimeManager, injectionMembers);
        }

        public static MessageHandlerFactoryForUnity RegisterType<T>(this MessageHandlerFactoryForUnity container, string name, params InjectionMember[] injectionMembers)
        {            
            return container.RegisterType(null, typeof(T), name, null, injectionMembers);
        }

        public static MessageHandlerFactoryForUnity RegisterType<T>(this MessageHandlerFactoryForUnity container, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {           
            return container.RegisterType(null, typeof(T), name, lifetimeManager, injectionMembers);
        }

        public static MessageHandlerFactoryForUnity RegisterType(this MessageHandlerFactoryForUnity container, Type t, params InjectionMember[] injectionMembers)
        {            
            return container.RegisterType(null, t, null, null, injectionMembers);
        }

        public static MessageHandlerFactoryForUnity RegisterType(this MessageHandlerFactoryForUnity container, Type from, Type to, params InjectionMember[] injectionMembers)
        {           
            return container.RegisterType(from, to, null, null, injectionMembers);
        }

        public static MessageHandlerFactoryForUnity RegisterType(this MessageHandlerFactoryForUnity container, Type from, Type to, string name, params InjectionMember[] injectionMembers)
        {           
            return container.RegisterType(from, to, name, null, injectionMembers);
        }

        public static MessageHandlerFactoryForUnity RegisterType(this MessageHandlerFactoryForUnity container, Type from, Type to, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {            
            return container.RegisterType(from, to, null, lifetimeManager, injectionMembers);
        }

        public static MessageHandlerFactoryForUnity RegisterType(this MessageHandlerFactoryForUnity container, Type t, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {           
            return container.RegisterType(null, t, null, lifetimeManager, injectionMembers);
        }

        public static MessageHandlerFactoryForUnity RegisterType(this MessageHandlerFactoryForUnity container, Type t, string name, params InjectionMember[] injectionMembers)
        {            
            return container.RegisterType(null, t, name, null, injectionMembers);
        }

        public static MessageHandlerFactoryForUnity RegisterType(this MessageHandlerFactoryForUnity container, Type t, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {            
            return container.RegisterType(null, t, name, lifetimeManager, injectionMembers);
        }

        public static MessageHandlerFactoryForUnity RegisterInstance<TInterface>(this MessageHandlerFactoryForUnity container, TInterface instance)
        {           
            return container.RegisterInstance(typeof(TInterface), null, instance, CreateDefaultInstanceLifetimeManager());
        }

        public static MessageHandlerFactoryForUnity RegisterInstance<TInterface>(this MessageHandlerFactoryForUnity container, TInterface instance, LifetimeManager lifetimeManager)
        {            
            return container.RegisterInstance(typeof(TInterface), null, instance, lifetimeManager);
        }

        public static MessageHandlerFactoryForUnity RegisterInstance<TInterface>(this MessageHandlerFactoryForUnity container, string name, TInterface instance)
        {            
            return container.RegisterInstance(typeof(TInterface), name, instance, CreateDefaultInstanceLifetimeManager());
        }

        public static MessageHandlerFactoryForUnity RegisterInstance<TInterface>(this MessageHandlerFactoryForUnity container, string name, TInterface instance, LifetimeManager lifetimeManager)
        {           
            return container.RegisterInstance(typeof(TInterface), name, instance, lifetimeManager);
        }

        public static MessageHandlerFactoryForUnity RegisterInstance(this MessageHandlerFactoryForUnity container, Type t, object instance)
        {            
            return container.RegisterInstance(t, null, instance, CreateDefaultInstanceLifetimeManager());
        }

        public static MessageHandlerFactoryForUnity RegisterInstance(this MessageHandlerFactoryForUnity container, Type t, object instance, LifetimeManager lifetimeManager)
        {            
            return container.RegisterInstance(t, null, instance, lifetimeManager);
        }

        public static MessageHandlerFactoryForUnity RegisterInstance(this MessageHandlerFactoryForUnity container, Type t, string name, object instance)
        {           
            return container.RegisterInstance(t, name, instance, CreateDefaultInstanceLifetimeManager());
        }

        public static T Resolve<T>(this MessageHandlerFactoryForUnity container, params ResolverOverride[] overrides)
        {            
            return (T) container.Resolve(typeof(T), null, overrides);
        }

        public static T Resolve<T>(this MessageHandlerFactoryForUnity container, string name, params ResolverOverride[] overrides)
        {            
            return (T) container.Resolve(typeof(T), name, overrides);
        }

        public static object Resolve(this MessageHandlerFactoryForUnity container, Type t, params ResolverOverride[] overrides)
        {            
            return container.Resolve(t, null, overrides);
        }

        public static IEnumerable<T> ResolveAll<T>(this MessageHandlerFactoryForUnity container, params ResolverOverride[] resolverOverrides)
        {
            return container.ResolveAll(typeof(T), resolverOverrides).Cast<T>();
        }

        public static T BuildUp<T>(this MessageHandlerFactoryForUnity container, T existing, params ResolverOverride[] resolverOverrides)
        {           
            return (T) container.BuildUp(typeof(T), existing, null, resolverOverrides);
        }

        public static T BuildUp<T>(this MessageHandlerFactoryForUnity container, T existing, string name, params ResolverOverride[] resolverOverrides)
        {            
            return (T) container.BuildUp(typeof(T), existing, name, resolverOverrides);
        }

        public static object BuildUp(this MessageHandlerFactoryForUnity container, Type t, object existing, params ResolverOverride[] resolverOverrides)
        {            
            return container.BuildUp(t, existing, null, resolverOverrides);
        }

        public static MessageHandlerFactoryForUnity AddNewExtension<TExtension>(this MessageHandlerFactoryForUnity container) where TExtension : UnityContainerExtension
        {            
            TExtension extension = UnityContainerExtensions.Resolve<TExtension>(container, new ResolverOverride[0]);
            return new MessageHandlerFactoryForUnity(container.AddExtension(extension));
        }

        public static TConfigurator Configure<TConfigurator>(this MessageHandlerFactoryForUnity container) where TConfigurator : IUnityContainerExtensionConfigurator
        {            
            return (TConfigurator)container.Configure(typeof(TConfigurator));
        }

        public static bool IsRegistered(this MessageHandlerFactoryForUnity container, Type typeToCheck)
        {            
            return UnityContainerExtensions.IsRegistered(container, typeToCheck, null);
        }

        public static bool IsRegistered(this MessageHandlerFactoryForUnity container, Type typeToCheck, string nameToCheck)
        {
            return container.Registrations.Any(registration => registration.RegisteredType == typeToCheck && registration.Name == nameToCheck);            
        }

        public static bool IsRegistered<T>(this MessageHandlerFactoryForUnity container)
        {            
            return UnityContainerExtensions.IsRegistered(container, typeof(T));
        }

        public static bool IsRegistered<T>(this MessageHandlerFactoryForUnity container, string nameToCheck)
        {            
            return UnityContainerExtensions.IsRegistered(container, typeof(T), nameToCheck);
        }

        private static LifetimeManager CreateDefaultInstanceLifetimeManager()
        {
            return new ContainerControlledLifetimeManager();
        }
    }
}
