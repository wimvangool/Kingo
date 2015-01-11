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

        private DependencyClass(Type concreteType, Type abstractType)
        {
            _concreteType = concreteType;
            _abstractType = abstractType;            
        }

        private void RegisterIn(MessageHandlerFactory factory, InstanceLifetime lifetime)
        {
            switch (lifetime)
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
                    throw MessageHandlerClass.NewInvalidLifetimeModeSpecifiedException(_concreteType, lifetime);
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

        internal static void RegisterDependencies(MessageHandlerFactory factory, Func<Type, bool> concreteTypePredicate)
        {
            var lifetimeCache = new Dictionary<Type, InstanceLifetime>();

            foreach (var dependency in FindDependencies(factory, lifetimeCache, concreteTypePredicate))
            {
                dependency.RegisterIn(factory, lifetimeCache[dependency._concreteType]);
            }
        }

        private static IEnumerable<DependencyClass> FindDependencies(MessageHandlerFactory factory, IDictionary<Type, InstanceLifetime> lifetimeCache, Func<Type, bool> concreteTypePredicate)
        {
            return from concreteType in FindConcreteTypes(factory, concreteTypePredicate)       
                   where HasDependencyAttribute(concreteType, lifetimeCache)
                   select new DependencyClass(concreteType, null);
        }        

        internal static void RegisterDependencies(MessageHandlerFactory factory, Func<Type, bool> concreteTypePredicate, Func<Type, bool> abstractTypePredicate)
        {
            var lifetimeCache = new Dictionary<Type, InstanceLifetime>();

            foreach (var dependency in FindDependencies(factory, lifetimeCache, concreteTypePredicate, abstractTypePredicate))
            {
                dependency.RegisterIn(factory, lifetimeCache[dependency._concreteType]);
            }
        }

        private static IEnumerable<DependencyClass> FindDependencies(MessageHandlerFactory factory, IDictionary<Type, InstanceLifetime> lifetimeCache, Func<Type, bool> concreteTypePredicate, Func<Type, bool> abstractTypePredicate)
        {            
            return from abstractType in FindAbstractTypes(factory, abstractTypePredicate)
                   from concreteType in FindConcreteTypes(factory, concreteTypePredicate)                   
                   where abstractType.IsAssignableFrom(concreteType) && HasDependencyAttribute(concreteType, lifetimeCache)
                   group concreteType by abstractType into typeMapping
                   where typeMapping.Any()
                   select CreateDependencyClass(typeMapping);                   
        }

        private static bool HasDependencyAttribute(Type concreteType, IDictionary<Type, InstanceLifetime> lifetimeCache)
        {
            if (lifetimeCache.ContainsKey(concreteType))
            {
                return true;
            }
            var dependencyAttribute = concreteType
                .GetCustomAttributes(typeof(DependencyAttribute), false)
                .Cast<DependencyAttribute>()
                .SingleOrDefault();

            if (dependencyAttribute == null)
            {
                return false;
            }
            lifetimeCache.Add(concreteType, dependencyAttribute.Lifetime);
            return true;
        }

        private static DependencyClass CreateDependencyClass(IGrouping<Type, Type> typeMapping)
        {            
            var abstractType = typeMapping.Key;
            var concreteType = GetConcreteType(typeMapping);            

            return new DependencyClass(concreteType, abstractType);
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
