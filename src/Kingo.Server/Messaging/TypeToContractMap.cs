using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Kingo.Messaging.Domain;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Provides a default implementation of the <see cref="ITypeToContractMap" /> interface.
    /// </summary>
    public class TypeToContractMap : ITypeToContractMap
    {        
        private readonly Lazy<Tuple<IReadOnlyDictionary<Type, string>, IReadOnlyDictionary<string, Type>>> _mappings;
        private readonly string _contractDelimiter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeToContractMap" /> class.
        /// </summary>
        /// <param name="typesToScan">A collection of types to scan for event and snapshot types.</param>
        /// <param name="contractDelimiter">Delimiter used to separate the namespace and name of a type in the contract.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="typesToScan"/> is <c>null</c>.
        /// </exception>
        public TypeToContractMap(IEnumerable<Type> typesToScan, string contractDelimiter = null)
        {
            if (typesToScan == null)
            {
                throw new ArgumentNullException("typesToScan");
            }
            _mappings = new Lazy<Tuple<IReadOnlyDictionary<Type, string>, IReadOnlyDictionary<string, Type>>>(() => CreateMapping(typesToScan), true);
            _contractDelimiter = string.IsNullOrEmpty(contractDelimiter) ? @"/" : contractDelimiter;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0} mapping(s) registered.", _mappings.Value.Item1.Count);
        }

        /// <inheritdoc />
        public string GetContract(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            string contract;

            if (_mappings.Value.Item1.TryGetValue(type, out contract))
            {
                return contract;
            }
            throw NewContractNotFoundException(type);
        }

        /// <inheritdoc />
        public Type GetType(string contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException("contract");
            }
            Type type;

            if (_mappings.Value.Item2.TryGetValue(contract, out type))
            {
                return type;
            }
            throw NewTypeNotFoundException(contract);
        }

        private Tuple<IReadOnlyDictionary<Type, string>, IReadOnlyDictionary<string, Type>> CreateMapping(IEnumerable<Type> typesToScan)
        {
            var typeToContractMapping = new Dictionary<Type, string>();
            var contractToTypeMapping = new Dictionary<string, Type>();

            foreach (var type in Filter(typesToScan))
            {
                MapToContract(type, typeToContractMapping, contractToTypeMapping);
            }
            return new Tuple<IReadOnlyDictionary<Type, string>, IReadOnlyDictionary<string, Type>>(typeToContractMapping, contractToTypeMapping);
        }

        /// <summary>
        /// Places a filter on the specified <paramref name="typesToScan"/>. The default implementation
        /// only selects non-abstract, non-generic class types.       
        /// </summary>
        /// <param name="typesToScan">The total set of types to scan.</param>
        /// <returns>A filtered collection of types to scan.</returns>
        protected virtual IEnumerable<Type> Filter(IEnumerable<Type> typesToScan)
        {
            return from type in typesToScan
                   where type != null && !type.IsAbstract && !type.IsGenericTypeDefinition && !type.ContainsGenericParameters
                   select type;
        }

        private void MapToContract(Type type, IDictionary<Type, string> typeToContractMapping, IDictionary<string, Type> contractToTypeMapping)
        {            
            if (typeToContractMapping.ContainsKey(type))
            {
                return;
            }
            var contract = MapToContract(type, _contractDelimiter);

            typeToContractMapping.Add(type, contract);

            try
            {
                contractToTypeMapping.Add(contract, type);
            }
            catch (ArgumentException)
            {
                throw new TypeToContractMapException(type, contractToTypeMapping[contract], contract);
            }
        }

        /// <summary>
        /// Maps a type to a specific contract.
        /// </summary>
        /// <param name="type">The type to map.</param>
        /// <param name="contractDelimiter">Delimiter used to separate the namespace and name of a type in the contract.</param>        
        /// <returns>The contract to which the specified <paramref name="type"/> is mapped.</returns>
        protected virtual string MapToContract(Type type, string contractDelimiter)
        {
            DataContractAttribute attribute;

            if (TryGetDataContractAttribute(type, out attribute))
            {
                return MapToContract(type, contractDelimiter, attribute);                
            }
            return type.Name;
        }   
     
        private static bool TryGetDataContractAttribute(Type type, out DataContractAttribute attribute)
        {
            return (attribute = type.GetCustomAttribute<DataContractAttribute>()) != null;
        }

        private static string MapToContract(Type type, string contractDelimiter, DataContractAttribute attribute)
        {
            var typeName = attribute.Name ?? type.Name;
            var typeNamespace = attribute.Namespace;

            if (string.IsNullOrEmpty(typeNamespace))
            {
                return typeName;
            }
            if (typeNamespace.EndsWith(contractDelimiter))
            {
                return typeNamespace + typeName;
            }
            return typeNamespace + contractDelimiter + typeName;
        }        

        private static Exception NewContractNotFoundException(Type type)
        {
            var messageFormat = ExceptionMessages.TypeToContractMap_ContractNotFound;
            var message = string.Format(messageFormat, type);
            return new ArgumentException(message, "type");
        }

        private static Exception NewTypeNotFoundException(string contract)
        {
            var messageFormat = ExceptionMessages.TypeToContractMap_TypeNotFound;
            var message = string.Format(messageFormat, contract);
            return new ArgumentException(message, "contract");
        }

        #region [====== Factory Methods ======]

        /// <summary>
        /// Scans all types from the <see cref="LayerConfiguration.ApiLayer" />, <see cref="LayerConfiguration.DomainLayer" />
        /// and <see cref="LayerConfiguration.DataAccessLayer" /> and auto-maps all messages and aggregates found in those
        /// layers.
        /// </summary>
        /// <param name="layers">A collection of logical application layers.</param>
        /// <param name="contractDelimiter">Delimiter used to separate the namespace and name of a type in the contract.</param>
        /// <returns>A mapping between all relevant types of the specified layers and their contract.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="layers"/> is <c>null</c>.
        /// </exception>
        public static TypeToContractMap FromLayerConfiguration(LayerConfiguration layers, string contractDelimiter = null)
        {
            if (layers == null)
            {
                throw new ArgumentNullException("layers");
            }
            var layersToScan = layers.ApiLayer + layers.DomainLayer + layers.DataAccessLayer;
            var typesToScan =
                from type in layersToScan
                where !type.IsAbstract && !type.ContainsGenericParameters
                where IsMessage(type) || IsAggregateRoot(type)
                select type;

            return new TypeToContractMap(typesToScan, contractDelimiter);
        }

        private static bool IsMessage(Type type)
        {
            return typeof(IMessage).IsAssignableFrom(type);
        }

        private static bool IsAggregateRoot(Type type)
        {
            var aggregateRootInterfaces =
                from interfaceType in type.GetInterfaces()
                where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IAggregateRoot<,>)
                select interfaceType;

            return aggregateRootInterfaces.Any();
        }

        #endregion

        #region [====== Built-in Maps ======]

        /// <summary>
        /// Represents an empty map.
        /// </summary>
        public static readonly ITypeToContractMap Empty = new TypeToContractMap(Enumerable.Empty<Type>());

        /// <summary>
        /// Represents a map that maps each type to its fully qualified name and back.
        /// </summary>
        public static readonly ITypeToContractMap FullyQualifiedName = new DelegateTypeToContractMap(ToFullyQualifiedName, FromFullyQualifiedName);

        private static string ToFullyQualifiedName(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        private static Type FromFullyQualifiedName(string contract)
        {
            return Type.GetType(contract);
        }

        #endregion
    }
}
