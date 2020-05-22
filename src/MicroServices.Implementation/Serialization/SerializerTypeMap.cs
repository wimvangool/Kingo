using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Serialization
{
    internal sealed class SerializerTypeMap
    {
        private readonly Dictionary<Type, Type> _serializerTypes;

        public SerializerTypeMap()
        {
            _serializerTypes = new Dictionary<Type, Type>();
        }

        public override string ToString() =>
            $"{_serializerTypes.Count} mapping(s) registered";

        public void Add(SerializerType serializerType, params Type[] associatedTypes) =>
            Add(serializerType, associatedTypes.AsEnumerable());

        public void Add(SerializerType serializerType, IEnumerable<Type> associatedTypes)
        {
            foreach (var associatedType in associatedTypes.Where(type => type != null && !type.IsInterface))
            {
                _serializerTypes[associatedType] = serializerType.Type;
            }
        }

        public ISerializer ResolveSerializer(Type associatedType, IServiceProvider serviceProvider)
        {
            if (TryResolveSerializerType(associatedType, out var serializerType))
            {
                var serializer = serviceProvider.GetService(serializerType);
                if (serializer != null)
                {
                    return serializer as ISerializer;
                }
            }
            return new JsonFormatSerializer();
        }

        private bool TryResolveSerializerType(Type associatedType, out Type serializerType)
        {
            if (associatedType == null || associatedType.IsInterface)
            {
                serializerType = null;
                return false;
            }
            if (_serializerTypes.TryGetValue(associatedType, out serializerType))
            {
                return true;
            }
            return TryResolveSerializerType(associatedType.BaseType, out serializerType);
        }
    }
}
