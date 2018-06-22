using System;
using System.Collections;
using System.Collections.Generic;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Basic implementation of the <see cref="ISchemaMap" /> interface.
    /// This map uses <see cref="StringComparer.OrdinalIgnoreCase" /> to compare type-identifiers.
    /// </summary>
    public sealed class SchemaMap : ISchemaMap, IReadOnlyDictionary<string, Type>, IReadOnlyDictionary<Type, string>
    {
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
