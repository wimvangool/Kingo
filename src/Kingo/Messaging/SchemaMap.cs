using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Basic implementation of the <see cref="ISchemaMap" /> interface.
    /// This map uses <see cref="StringComparer.OrdinalIgnoreCase" /> to compare type-identifiers.
    /// </summary>
    public class SchemaMap : ISchemaMap, IReadOnlyDictionary<string, Type>, IReadOnlyDictionary<Type, string>
    {
        #region [====== NullMap ======]

        private sealed class NullMap : ISchemaMap
        {
            public Type GetType(string typeId) =>
                throw NewTypeNotFoundException(typeId, null);

            public string GetTypeId(Type type) =>
                throw NewTypeIdNotFoundException(type, null);
        }

        #endregion

        public static readonly ISchemaMap None = new NullMap();
        private readonly Dictionary<string, Type> _identifierToTypeMap;
        private readonly Dictionary<Type, string> _typeToIdentifierMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaMap" /> class.
        /// </summary>
        public SchemaMap()
        {
            _identifierToTypeMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            _typeToIdentifierMap = new Dictionary<Type, string>();
        }

        /// <summary>
        /// Returns the number of mappings in this map.
        /// </summary>
        public int Count =>
            _identifierToTypeMap.Count;

        /// <inheritdoc />
        public override string ToString() =>
            $"{ Count } mapping(s)";

        #region [====== Add ======]       

        /// <summary>
        /// Adds a mapping for each type from the assemblies that match the specified search criteria that is non-abstract, decorated with the <see cref="DataContractAttribute"/>
        /// and satisfies the specified <paramref name="predicate" />. The type-id is derived from the information provided by the <see cref="DataContractAttribute"/>.
        /// </summary>
        /// <param name="searchPattern">The pattern that is used to match specified files/assemblies.</param>        
        /// <param name="predicate">Optional predicate that is used to filter specific types from the assemblies.</param>
        /// <returns>The map that contains all added mappings.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="searchPattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The specified collection causes a specific type or type-id to be added more than once to this map.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public SchemaMap AddDataContracts(string searchPattern, Func<Type, bool> predicate) =>
            AddDataContracts(searchPattern, null, predicate);

        /// <summary>
        /// Adds a mapping for each type from the assemblies that match the specified search criteria that is non-abstract, decorated with the <see cref="DataContractAttribute"/>
        /// and satisfies the specified <paramref name="predicate" />. The type-id is derived from the information provided by the <see cref="DataContractAttribute"/>.
        /// </summary>
        /// <param name="searchPattern">The pattern that is used to match specified files/assemblies.</param>
        /// <param name="path">A path pointing to a specific directory. If <c>null</c>, the <see cref="TypeSet.CurrentDirectory"/> is used.</param>
        /// <param name="predicate">Optional predicate that is used to filter specific types from the assemblies.</param>
        /// <returns>The map that contains all added mappings.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="searchPattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The specified collection causes a specific type or type-id to be added more than once to this map.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public SchemaMap AddDataContracts(string searchPattern, string path = null, Func<Type, bool> predicate = null)
        {
            var types =
                from type in TypeSet.Empty.Add(searchPattern, path)
                where predicate == null || predicate.Invoke(type)
                select type;

            return AddDataContracts(types);
        }

        /// <summary>
        /// Adds a mapping for each type in <paramref name="types"/> that is non-abstract and is decorated with the <see cref="DataContractAttribute"/>.
        /// The type-id is derived from the information provided by the <see cref="DataContractAttribute"/>.
        /// </summary>
        /// <param name="types">A collection of types.</param>
        /// <returns>The map that contains all added mappings.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The specified collection causes a specific type or type-id to be added more than once to this map.
        /// </exception>
        public SchemaMap AddDataContracts(IEnumerable<Type> types) =>
            Add(types, GetTypeIdFromDataContract);

        private static string GetTypeIdFromDataContract(Type type)
        {
            if (type.IsAbstract)
            {
                return null;
            }
            var attribute = type.GetCustomAttribute(typeof(DataContractAttribute), false) as DataContractAttribute;
            if (attribute == null)
            {
                return null;
            }
            return attribute.Namespace + (attribute.Name ?? type.FriendlyName());
        }        

        /// <summary>
        /// Iterates over the specified <paramref name="types"/> and obtains the associated type-id for each type
        /// by calling the specified <paramref name="typeIdFactory" />. If this delegate returns <c>null</c>, the type
        /// is ignored; otherwise, the type and resulting type-id mapping are added to this map.
        /// </summary>
        /// <param name="types">A collection of types.</param>
        /// <param name="typeIdFactory">Delegate that is used to obtain a unique type-id for a specific type.</param>
        /// <returns>The map that contains all added mappings.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> or <paramref name="typeIdFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The operation causes a specific type or type-id to be added more than once to this map.
        /// </exception>
        public SchemaMap Add(IEnumerable<Type> types, Func<Type, string> typeIdFactory)
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }
            if (typeIdFactory == null)
            {
                throw new ArgumentNullException(nameof(typeIdFactory));
            }
            var map = this;

            foreach (var type in types.WhereNotNull())
            {
                var typeId = typeIdFactory.Invoke(type);
                if (typeId != null)
                {
                    map = map.Add(typeId, type);
                }
            }
            return map;
        }

        /// <summary>
        /// Adds the specified mapping to this map.
        /// </summary>
        /// <param name="typeId">Unique identifier of the specified <paramref name="type"/>.</param>
        /// <param name="type">A type.</param>
        /// <returns>A schema-map that contains the specified mapping.</returns>
        /// <exception cref="ArgumentNullException">
        /// Either <paramref name="typeId"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Either <paramref name="typeId"/> or <paramref name="type"/> is already mapped in this map.
        /// </exception>
        public SchemaMap Add(string typeId, Type type)
        {
            if (typeId == null)
            {
                throw new ArgumentNullException(nameof(typeId));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (_identifierToTypeMap.ContainsKey(typeId))
            {
                throw NewTypeIdAlreadyMappedException(typeId);
            }
            if (_typeToIdentifierMap.ContainsKey(type))
            {
                throw NewTypeAlreadyMappedException(type);
            }
            _identifierToTypeMap.Add(typeId, type);
            _typeToIdentifierMap.Add(type, typeId);
            return this;
        }        

        private static Exception NewTypeIdAlreadyMappedException(string typeId)
        {
            var messageFormat = ExceptionMessages.SchemaMap_TypeIdAlreadyMapped;
            var message = string.Format(messageFormat, typeId);
            return new ArgumentException(message, nameof(typeId));
        }

        private static Exception NewTypeAlreadyMappedException(Type type)
        {
            var messageFormat = ExceptionMessages.SchemaMap_TypeAlreadyMapped;
            var message = string.Format(messageFormat, type.FriendlyName());
            return new ArgumentException(message, nameof(type));
        }

        #endregion

        #region [====== ISchemaMap ======]

        /// <inheritdoc />
        public Type GetType(string typeId)
        {
            if (typeId == null)
            {
                throw new ArgumentNullException(nameof(typeId));
            }
            try
            {
                return _identifierToTypeMap[typeId];
            }
            catch (KeyNotFoundException exception)
            {
                throw NewTypeNotFoundException(typeId, exception);
            }
        }        

        /// <inheritdoc />
        public string GetTypeId(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            try
            {
                return _typeToIdentifierMap[type];
            }
            catch (KeyNotFoundException exception)
            {
                throw NewTypeIdNotFoundException(type, exception);
            }
        }       

        private static Exception NewTypeNotFoundException(string typeId, Exception exception)
        {
            var messageFormat = ExceptionMessages.SchemaMap_TypeNotFound;
            var message = string.Format(messageFormat, typeId);
            return new ArgumentException(message, nameof(typeId), exception);
        }

        private static Exception NewTypeIdNotFoundException(Type type, Exception exception)
        {
            var messageFormat = ExceptionMessages.SchemaMap_TypeIdNotFound;
            var message = string.Format(messageFormat, type.FriendlyName());
            return new ArgumentException(message, nameof(type), exception);
        }

        #endregion

        #region [====== IReadOnlyDictionary implementations ======]                

        Type IReadOnlyDictionary<string, Type>.this[string key] =>
            _identifierToTypeMap[key];

        string IReadOnlyDictionary<Type, string>.this[Type key] =>
            _typeToIdentifierMap[key];

        IEnumerable<string> IReadOnlyDictionary<string, Type>.Keys =>
            _identifierToTypeMap.Keys;

        IEnumerable<Type> IReadOnlyDictionary<Type, string>.Keys =>
            _typeToIdentifierMap.Keys;

        IEnumerable<Type> IReadOnlyDictionary<string, Type>.Values =>
            _identifierToTypeMap.Values;

        IEnumerable<string> IReadOnlyDictionary<Type, string>.Values =>
            _typeToIdentifierMap.Values;

        bool IReadOnlyDictionary<string, Type>.ContainsKey(string key) =>
            _identifierToTypeMap.ContainsKey(key);

        bool IReadOnlyDictionary<Type, string>.ContainsKey(Type key) =>
            _typeToIdentifierMap.ContainsKey(key);

        bool IReadOnlyDictionary<string, Type>.TryGetValue(string key, out Type value) =>
            _identifierToTypeMap.TryGetValue(key, out value);

        bool IReadOnlyDictionary<Type, string>.TryGetValue(Type key, out string value) =>
            _typeToIdentifierMap.TryGetValue(key, out value);

        IEnumerator<KeyValuePair<string, Type>> IEnumerable<KeyValuePair<string, Type>>.GetEnumerator() =>
            _identifierToTypeMap.GetEnumerator();

        IEnumerator<KeyValuePair<Type, string>> IEnumerable<KeyValuePair<Type, string>>.GetEnumerator() =>
            _typeToIdentifierMap.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var identifierToTypeMapping in _identifierToTypeMap)
            {
                yield return identifierToTypeMapping;
            }
            foreach (var typeToIdentifierMapping in _typeToIdentifierMap)
            {
                yield return typeToIdentifierMapping;
            }
        }

        #endregion
    }
}
