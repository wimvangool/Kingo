using System;

namespace Kingo.Serialization
{
    internal sealed class SerializerFactory : ISerializerFactory
    {
        private readonly SerializerTypeMap _serializerTypeMap;
        private readonly IServiceProvider _serviceProvider;

        public SerializerFactory(SerializerTypeMap serializerTypeMap, IServiceProvider serviceProvider)
        {
            _serializerTypeMap = serializerTypeMap;
            _serviceProvider = serviceProvider;
        }

        public override string ToString() =>
            _serializerTypeMap.ToString();

        public ISerializer CreateSerializerFor(Type associatedType) =>
            _serializerTypeMap.ResolveSerializer(associatedType, _serviceProvider);
    }
}
