using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Server
{
    internal sealed class DependencyClass
    {
        #region [====== Instance Members ======]

        private readonly Type _concreteType;
        private readonly Type _abstractType; 
        private readonly InstanceLifetime _lifetime;

        private DependencyClass(Type concreteType, Type abstractType, InstanceLifetime lifetime)
        {
            _concreteType = concreteType;
            _abstractType = abstractType;
            _lifetime = lifetime;
        }

        private void RegisterIn(MessageHandlerFactory factory)
        {
            switch (_lifetime)
            {
                case InstanceLifetime.PerResolve:
                    RegisterPerResolve(factory);
                    return;

                case InstanceLifetime.PerUnitOfWork:
                    RegisterPerUnitOfWork(factory);
                    return;

                case InstanceLifetime.PerScenario:
                    RegisterPerScenario(factory);
                    return;

                case InstanceLifetime.Singleton:
                    RegisterSingleton(factory);
                    return;

                default:
                    throw MessageHandlerClass.NewInvalidLifetimeModeSpecifiedException(_concreteType, _lifetime);
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

        private void RegisterPerScenario(MessageHandlerFactory factory)
        {
            if (_abstractType == null)
            {
                factory.RegisterWithPerScenarioLifetime(_concreteType);
            }
            else
            {
                factory.RegisterWithPerScenarioLifetime(_concreteType, _abstractType);
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

        internal static void RegisterDependencies(MessageHandlerFactory factory, Func<Type, bool> concreteTypePredicate, InstanceLifetime defaultLifetime)
        {
            foreach (var dependency in FindDependencies(factory, concreteTypePredicate, defaultLifetime))
            {
                dependency.RegisterIn(factory);
            }
        }

        private static IEnumerable<DependencyClass> FindDependencies(MessageHandlerFactory factory, Func<Type, bool> concreteTypePredicate, InstanceLifetime defaultLifetime)
        {
            return from concreteType in FindConcreteTypes(factory, concreteTypePredicate)
                   let lifetime = GetSpecifiedOrDefaultLifetime(concreteType, defaultLifetime)
                   select new DependencyClass(concreteType, null, lifetime);
        }        

        internal static void RegisterDependencies(MessageHandlerFactory factory, Func<Type, bool> concreteTypePredicate, Func<Type, bool> abstractTypePredicate, InstanceLifetime defaultLifetime)
        {
            foreach (var dependency in FindDependencies(factory, concreteTypePredicate, abstractTypePredicate, defaultLifetime))
            {
                dependency.RegisterIn(factory);
            }
        }

        private static IEnumerable<DependencyClass> FindDependencies(MessageHandlerFactory factory, Func<Type, bool> concreteTypePredicate, Func<Type, bool> abstractTypePredicate, InstanceLifetime defaultLifetime)
        {
            var lifetimeCache = new Dictionary<Type, InstanceLifetime>();

            return from abstractType in FindAbstractTypes(factory, abstractTypePredicate)
                   from concreteType in FindConcreteTypes(factory, concreteTypePredicate)
                   where abstractType.IsAssignableFrom(concreteType)
                   group concreteType by abstractType into typeMapping
                   where typeMapping.Any()
                   select CreateDependencyClass(typeMapping, defaultLifetime, lifetimeCache);                   
        }

        private static DependencyClass CreateDependencyClass(IGrouping<Type, Type> typeMapping, InstanceLifetime defaultLifetime, IDictionary<Type, InstanceLifetime> lifetimeCache)
        {            
            var abstractType = typeMapping.Key;
            var concreteType = GetConcreteType(typeMapping);
            var lifetime = GetInstanceLifetimeOf(concreteType, defaultLifetime, lifetimeCache);

            return new DependencyClass(concreteType, abstractType, lifetime);
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

        private static InstanceLifetime GetInstanceLifetimeOf(Type concreteType, InstanceLifetime defaultLifetime, IDictionary<Type, InstanceLifetime> lifetimeCache)
        {
            InstanceLifetime lifetime;

            if (lifetimeCache.TryGetValue(concreteType, out lifetime))
            {
                return lifetime;
            }            
            lifetimeCache.Add(concreteType, lifetime = GetSpecifiedOrDefaultLifetime(concreteType, defaultLifetime));

            return lifetime;
        }

        private static InstanceLifetime GetSpecifiedOrDefaultLifetime(Type concreteType, InstanceLifetime defaultLifetime)
        {
            var attribute = concreteType.GetCustomAttributes(typeof(InstanceLifetimeAttribute), false).SingleOrDefault() as InstanceLifetimeAttribute;
            if (attribute == null)
            {
                return defaultLifetime;
            }
            return attribute.Lifetime;
        }

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

        private static IEnumerable<Type> FindAbstractTypes(MessageHandlerFactory factory, Func<Type, bool> predicate)
        {
            return from type in AssemblySet.Join(factory.ApplicationLayer, factory.DomainLayer).GetTypes()
                   where IsPublicAbstractType(type) && SatisfiesPredicate(type, predicate)
                   select type;
        }

        private static bool IsPublicAbstractType(Type type)
        {
            return type.IsPublic && ((type.IsClass && type.IsAbstract) || type.IsInterface);
        }

        private static IEnumerable<Type> FindConcreteTypes(MessageHandlerFactory factory, Func<Type, bool> predicate)
        {
            return from type in AssemblySet.Join(factory.ServiceLayer, factory.DataAccessLayer).GetTypes()
                   where IsPublicConcreteType(type) && SatisfiesPredicate(type, predicate)
                   select type;
        }

        private static bool IsPublicConcreteType(Type type)
        {
            return type.IsPublic && type.IsClass && !type.IsAbstract;
        }

        private static bool SatisfiesPredicate(Type type, Func<Type, bool> predicate)
        {
            return predicate == null || predicate.Invoke(type);
        }

        #endregion
    }
}
