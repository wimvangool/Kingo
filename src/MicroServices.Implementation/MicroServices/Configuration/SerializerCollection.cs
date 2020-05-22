using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kingo.Reflection;
using Kingo.Serialization;
using Microsoft.Extensions.DependencyInjection;
using static Kingo.Ensure;

namespace Kingo.MicroServices.Configuration
{
    /// <summary>
    /// Represents a collection of serializers that can be used by a processor to serialize and deserialize
    /// all kinds of (data) objects.
    /// </summary>
    public sealed class SerializerCollection : IMicroProcessorComponentCollection
    {
        #region [====== NullTypeMapping ======]

        private sealed class NullTypeMapping : ISerializerTypeMapping
        {
            private readonly Type _type;

            public NullTypeMapping(Type type)
            {
                _type = type;
            }

            public ISerializerTypeMapping For(IEnumerable<Type> associatedTypes) =>
                throw NewTypeNotSerializerException(_type);

            private static Exception NewTypeNotSerializerException(Type type)
            {
                var messageFormat = ExceptionMessages.SerializerCollection_TypeNoSerializerType;
                var message = string.Format(messageFormat, type.FriendlyName(), nameof(ISerializer));
                return new InvalidOperationException(message);
            }
        }

        #endregion

        #region [====== SerializerTypeMapping ======]

        private sealed class SerializerTypeMapping : ISerializerTypeMapping
        {
            private readonly SerializerCollection _serializers;
            private readonly SerializerType _serializerType;
            private readonly IEnumerable<Type> _associatedTypes;

            public SerializerTypeMapping(SerializerCollection serializers, SerializerType serializerType)
            {
                _serializers = serializers;
                _serializerType = serializerType;
                _associatedTypes = Enumerable.Empty<Type>();
            }

            public SerializerType SerializerType =>
                _serializerType;

            private SerializerTypeMapping(SerializerCollection serializers, SerializerType serializerType, IEnumerable<Type> associatedTypes)
            {
                _serializers = serializers;
                _serializerType = serializerType;
                _associatedTypes = associatedTypes;
            }

            public ISerializerTypeMapping For(IEnumerable<Type> associatedTypes)
            {
                var allAssociatedTypes = _associatedTypes.Concat(IsNotNull(associatedTypes, nameof(associatedTypes)).ToArray());
                var mapping = new SerializerTypeMapping(_serializers, _serializerType, allAssociatedTypes);
                var mappingIndex = _serializers._serializerTypeMappings.IndexOf(this);
                if (mappingIndex < 0)
                {
                    _serializers._serializerTypeMappings.Add(mapping);
                }
                else
                {
                    _serializers._serializerTypeMappings[mappingIndex] = mapping;
                }
                return mapping;
            }

            public void AddTo(SerializerTypeMap serializerTypeMap) =>
                serializerTypeMap.Add(_serializerType, _associatedTypes);
        }

        #endregion

        private readonly List<SerializerTypeMapping> _serializerTypeMappings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerCollection" /> class.
        /// </summary>
        public SerializerCollection()
        {
            _serializerTypeMappings = new List<SerializerTypeMapping>();
        }

        #region [====== IEnumerable<MicroProcessorComponent> ======]

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<MicroProcessorComponent> GetEnumerator() =>
            _serializerTypeMappings.Select(mapping => mapping.SerializerType).GetEnumerator();

        #endregion

        #region [====== AddSpecificComponentsTo ======]

        IServiceCollection IMicroProcessorComponentCollection.AddSpecificComponentsTo(IServiceCollection services) =>
            AddSerializerFactory(services, CreateSerializerTypeMap());

        private IServiceCollection AddSerializerFactory(IServiceCollection services, SerializerTypeMap serializerTypeMap) =>
            services.AddTransient<ISerializerFactory>(serviceProvider => new SerializerFactory(serializerTypeMap, serviceProvider));

        private SerializerTypeMap CreateSerializerTypeMap()
        {
            var serializerTypeMap = new SerializerTypeMap();

            foreach (var serializerTypeMapping in _serializerTypeMappings)
            {
                serializerTypeMapping.AddTo(serializerTypeMap);
            }
            return serializerTypeMap;
        }

        #endregion

        /// <summary>
        /// Adds the specified <paramref name="type"/> to the collection if and only if the type
        /// represents a concrete type that implements the <see cref="ISerializer"/> interface.
        /// </summary>
        /// <param name="type">The type to add.</param>
        /// <returns>A mapping that can be used to associated other dependencies with the serializer.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public ISerializerTypeMapping Add(Type type)
        {
            if (SerializerType.IsSerializerType(IsNotNull(type, nameof(type)), out var serializerType))
            {
                return new SerializerTypeMapping(this, serializerType);
            }
            return new NullTypeMapping(type);
        }
    }
}
