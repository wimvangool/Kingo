using System;
using Kingo.Serialization;
using static Kingo.Ensure;

namespace Kingo.MicroServices.DataContracts
{
    internal sealed class DataContractSerializer : IDataContractSerializer
    {
        private readonly ISerializer _serializer;
        private readonly DataContractTypeMap _typeMap;

        public DataContractSerializer(ISerializer serializer, DataContractTypeMap typeMap)
        {
            _serializer = serializer;
            _typeMap = typeMap;
        }

        #region [====== Serialize ======]

        public DataContractBlob Serialize(object content) =>
            Serialize(content, _typeMap.GetContentType(IsNotNull(content, nameof(content)).GetType()));

        private DataContractBlob Serialize(object content, DataContractType contentType)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [====== Deserialize ======]

        public object Deserialize(byte[] content, string contentType, bool updateToLatestVersion = false) =>
            Deserialize(DataContractBlob.FromBytes(contentType, content), updateToLatestVersion);

        public object Deserialize(DataContractBlob blob, bool updateToLatestVersion = false) =>
            throw new NotImplementedException();

        #endregion
    }
}
