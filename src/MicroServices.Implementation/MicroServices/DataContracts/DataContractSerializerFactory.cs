using System;
using Kingo.Serialization;

namespace Kingo.MicroServices.DataContracts
{
    internal sealed class DataContractSerializerFactory : IDataContractSerializerFactory
    {
        private readonly ISerializerFactory _serializerFactory;
        private readonly DataContractTypeMap _dataContractTypeMap;

        public DataContractSerializerFactory(ISerializerFactory serializerFactory, DataContractTypeMap dataContractTypeMap)
        {
            _serializerFactory = serializerFactory;
            _dataContractTypeMap = dataContractTypeMap;
        }

        public IDataContractSerializer CreateSerializer(Type associatedType) =>
            new DataContractSerializer(_serializerFactory.CreateSerializerFor(associatedType), _dataContractTypeMap);
    }
}
