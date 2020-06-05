using System;
using System.Collections.Generic;
using Kingo.Serialization;
using static Kingo.Ensure;

namespace Kingo.MicroServices.DataContracts
{
    internal sealed class DataContractSerializer : IDataContractSerializer
    {
        private readonly ISerializer _serializer;
        private readonly DataContractTypeMap _typeMap;

        public DataContractSerializer(ISerializer serializer = null, DataContractTypeMap typeMap = null)
        {
            _serializer = serializer ?? new JsonFormatSerializer();
            _typeMap = typeMap ?? new DataContractTypeMap();
        }

        #region [====== Serialize ======]

        public DataContractBlob Serialize(object content) =>
            Serialize(content, _typeMap.GetContentTypeOf(IsNotNull(content, nameof(content)).GetType()));

        private DataContractBlob Serialize(object content, DataContractContentType contentType) =>
            DataContractBlob.FromBytes(contentType, _serializer.Serialize(content));

        #endregion

        #region [====== Deserialize ======]

        public object Deserialize(IReadOnlyList<byte> content, string contentType, bool updateToLatestVersion = false) =>
            Deserialize(DataContractBlob.FromBytes(contentType, content), updateToLatestVersion);

        public object Deserialize(DataContractBlob blob, bool updateToLatestVersion = false) =>
            Deserialize(blob, _typeMap.GetTypeOf(IsNotNull(blob, nameof(blob)).ContentType), updateToLatestVersion);

        private object Deserialize(DataContractBlob blob, Type type, bool updateToLatestVersion = false)
        {
            var content = _serializer.Deserialize(blob.Content.ToArray(), type);

            return content;
        }

        #endregion
    }
}
