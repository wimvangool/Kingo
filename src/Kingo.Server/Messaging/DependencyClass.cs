using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kingo.Resources;

namespace Kingo.Messaging
{
    internal sealed class DependencyClass
    {
        #region [====== Instance Members ======]

        private readonly Type _concreteType;
        private readonly Type _abstractType;
        private readonly IDependencyConfiguration _configuration; 

        private DependencyClass(Type concreteType, Type abstractType, IDependencyConfiguration configuration)
        {
            _concreteType = concreteType;
            _abstractType = abstractType;
            _configuration = configuration;
        }

        private void RegisterIn(MessageHandlerFactory factory)
        {
            switch (_configuration.Lifetime)
            {
                case InstanceLifetime.PerResolve:
                    RegisterPerResolve(factory);
                    return;

                case InstanceLifetime.PerUnitOfWork:
                    RegisterPerUnitOfWork(factory);
                    return;                

                case InstanceLifetime.Singleton:
                    RegisterSingleton(factory);
                    return;

                default:
                    throw MessageHandlerClass.NewInvalidLifetimeModeSpecifiedException(_concreteType, _configuration.Lifetime);
            }
        }

        private void RegisterPerResolve(MessageHandlerFactory factory)
        {
            if (_abstractType == null)
            {
                factory.RegisterWithPerResolveLifetime(_concreteType);
            }
            else
            {
                factory.RegisterWithPerResolveLifetime(_concreteType, _abstractType);
            }
        }

        private void RegisterPerUnitOfWork(MessageHandlerFactory factory)
        {
            if (_abstractType == null)
            {
                factory.RegisterWithPerUnitOfWorkLifetime(_concreteType);
            }
            else
            {
                factory.RegisterWithPerUnitOfWorkLifetime(_concreteType, _abstractType);
            }
        }        

        private void RegisterSingleton(MessageHandlerFactory factory)
        {
            if (_abstractType == null)
            {
                factory.RegisterSingleton(_concreteType);
            }
            else
            {
                factory.RegisterSingleton(_concreteType, _abstractType);
            }
        }

        #endregion

        #region [====== Static Members ======]

        internal static void RegisterDependencies(MessageHandlerFactory factory, AssemblySet assemblies, Predicate<Type> concreteTypePredicate, Func<Type, IDependencyConfiguration> configurationFactory)
        {            
            if (assemblies == null)
            {
                throw new ArgumentNullException("assemblies");
            }
            if (concreteTypePredicate == null)
            {
                throw new ArgumentNullException("concreteTypePredicate");
            }
            foreach (var dependency in FindDependencies(assemblies, concreteTypePredicate, configurationFactory))
            {
                dependency.RegisterIn(factory);
            }
        }

        private static IEnumerable<DependencyClass> FindDependencies(AssemblySet assemblies, Predicate<Type> concreteTypePredicate, Func<Type, IDependencyConfiguration> configurationFactory)
        {
            return from concreteType in FindConcreteTypes(assemblies, concreteTypePredicate)   
                   let configuration = DetermineConfigurationOf(concreteType, configurationFactory)    
                   select new DependencyClass(concreteType, null, configuration);
        }

        internal static void RegisterDependencies(MessageHandlerFactory factory, AssemblySet assemblies, Predicate<Type> concreteTypePredicate, Predicate<Type> abstractTypePredicate, Func<Type, IDependencyConfiguration> configurationFactory)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException("assemblies");
            }           
            if (abstractTypePredicate == null)
            {
                throw new ArgumentNullException("abstractTypePredicate");
            }
            foreach (var dependency in FindDependencies(assemblies, concreteTypePredicate, abstractTypePredicate, configurationFactory))
            {
                dependency.RegisterIn(factory);
            }
        }

        private static IEnumerable<DependencyClass> FindDependencies(AssemblySet assemblies, Predicate<Type> concreteTypePredicate, Predicate<Type> abstractTypePredicate, Func<Type, IDependencyConfiguration> configurationFactory)
        {            
            return from abstractType in FindAbstractTypes(assemblies, abstractTypePredicate)
                   from concreteType in FindConcreteTypes(assemblies, concreteTypePredicate)                   
                   where abstractType.IsAssignableFrom(concreteType)
                   group concreteType by abstractType into typeMapping
                   where typeMapping.Any()
                   select CreateDependencyClass(typeMapping, configurationFactory);                   
        }

        private static DependencyClass CreateDependencyClass(IGrouping<Type, Type> typeMapping, Func<Type, IDependencyConfiguration> configurationFactory)
        {            
            var abstractType = typeMapping.Key;
            var concreteType = GetConcreteType(typeMapping);
            var configuration = DetermineConfigurationOf(concreteType, configurationFactory);

            return new DependencyClass(concreteType, abstractType, configuration);
        }

        private static Type GetConcreteType(IGrouping<Type, Type> typeMapping)
        {
            try
            {
                return typeMapping.Single();
            }
            catch (InvalidOperationException)
            {
                throw NewAmbiguousMatchException(typeMapping.Key, typeMapping);
            }
        }

        private static IEnumerable<Type> FindAbstractTypes(AssemblySet assemblies, Predicate<Type> predicate)
        {
            return from type in assemblies.GetTypes()
                   where IsPublicAbstractType(type) && SatisfiesPredicate(type, predicate)
                   select type;            
        }

        private static bool IsPublicAbstractType(Type type)
        {
            return type.IsPublic && ((type.IsClass && type.IsAbstract) || type.IsInterface);
        }

        private static IEnumerable<Type> FindConcreteTypes(AssemblySet assemblies, Predicate<Type> predicate)
        {
            return from type in assemblies.GetTypes()
                   where IsPublicConcreteType(type) && SatisfiesPredicate(type, predicate)
                   select type;           
        }

        private static bool IsPublicConcreteType(Type type)
        {
            return type.IsPublic && type.IsClass && !type.IsAbstract;
        }

        private static bool SatisfiesPredicate(Type type, Predicate<Type> predicate)
        {
            return predicate == null || predicate.Invoke(type);
        }

        private static IDependencyConfiguration DetermineConfigurationOf(Type concreteType, Func<Type, IDependencyConfiguration> configurationFactory)
        {
            // If no factory is specified or the provided factory does not return a configuration for this type,
            // we try to obtain the configuration through the DependencyAttribute. If this attribute is
            // not declared on the specified type, we fall back to the default configuration.
            IDependencyConfiguration configuration;

            if (configurationFactory == null || (configuration = configurationFactory.Invoke(concreteType)) == null)
            {
                if (TryGetDependencyAttribute(concreteType, out configuration))
                {
                    return configuration;
                }
                return DependencyConfiguration.Default;
            }            
            return configuration;
        }

        private static bool TryGetDependencyAttribute(Type concreteType, out IDependencyConfiguration configuration)
        {
            configuration = concreteType
                .GetCustomAttributes(typeof(DependencyAttribute), false)
                .Cast<DependencyAttribute>()
                .SingleOrDefault();

            return configuration != null;
        }

        #endregion

        #region [====== Exception Factory Methods ======]

        private static Exception NewAmbiguousMatchException(Type abstractType, IEnumerable<Type> concreteTypes)
        {
            var messageFormat = ExceptionMessages.DependencyClass_AmbigiousMatch;
            var message = string.Format(messageFormat, abstractType.Name, ToString(concreteTypes));
            return new ArgumentException(message);
        }

        private static string ToString(IEnumerable<Type> concreteTypes)
        {
            var types = new StringBuilder();

            foreach (var type in concreteTypes)
            {
                types.AppendFormat("[{0}]", type.Name);
            }
            return types.ToString();
        }

        #endregion
    }
}
