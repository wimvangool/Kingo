using System;
using System.Collections.Generic;
using Kingo.Reflection;

namespace Kingo.MicroServices.DataContracts
{
    internal sealed class DataContractTypeMap
    {
        private readonly Dictionary<Type, DataContractContentType> _typeToContentTypeMap;
        private readonly Dictionary<DataContractContentType, Type> _contentTypeToTypeMap;

        public DataContractTypeMap()
        {
            _typeToContentTypeMap = new Dictionary<Type, DataContractContentType>();
            _contentTypeToTypeMap = new Dictionary<DataContractContentType, Type>();
        }

        public override string ToString() =>
            $"{_typeToContentTypeMap.Count} data-contract(s) registered";

        public void Add(DataContractType dataContractType)
        {
            // To ensure there is only a struct 1-to-1 mapping between types and content-types,
            // we must always add or replace existing mapping both ways in the type-map.
            if (_typeToContentTypeMap.TryGetValue(dataContractType.Type, out var contentType))
            {
                _typeToContentTypeMap.Remove(dataContractType.Type);
                _contentTypeToTypeMap.Remove(contentType);
            }
            else if (_contentTypeToTypeMap.TryGetValue(dataContractType.ContentType, out var type))
            {
                _typeToContentTypeMap.Remove(type);
                _contentTypeToTypeMap.Remove(dataContractType.ContentType);
            }
            _typeToContentTypeMap.Add(dataContractType.Type, dataContractType.ContentType);
            _contentTypeToTypeMap.Add(dataContractType.ContentType, dataContractType.Type);
        }

        public DataContractContentType GetContentTypeOf(Type type)
        {
            try
            {
                return _typeToContentTypeMap[type];
            }
            catch (KeyNotFoundException exception)
            {
                throw NewContentTypeNotMappedException(type, exception);
            }
        }

        public Type GetTypeOf(DataContractContentType contentType)
        {
            try
            {
                return _contentTypeToTypeMap[contentType];
            }
            catch (KeyNotFoundException exception)
            {
                throw NewTypeNotMappedException(contentType, exception);
            }
        }

        private static Exception NewContentTypeNotMappedException(Type type, Exception exception)
        {
            var messageFormat = ExceptionMessages.DataContractTypeMap_ContentTypeNotMapped;
            var message = string.Format(messageFormat, type.FriendlyName());
            return new ArgumentException(message, nameof(type));
        }

        private static Exception NewTypeNotMappedException(DataContractContentType contentType, Exception exception)
        {
            var messageFormat = ExceptionMessages.DataContractTypeMap_TypeNotMapped;
            var message = string.Format(messageFormat, contentType);
            return new ArgumentException(message, nameof(contentType));
        }
    }
}
