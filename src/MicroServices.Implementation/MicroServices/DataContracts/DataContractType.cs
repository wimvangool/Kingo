using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Kingo.Reflection;

namespace Kingo.MicroServices.DataContracts
{
    internal sealed class DataContractType : MicroProcessorComponent
    {
        private readonly DataContractContentType _contentType;

        private DataContractType(MicroProcessorComponent component, DataContractContentType contentType) : base(component, Enumerable.Empty<Type>())
        {
            _contentType = contentType;
        }

        public DataContractContentType ContentType =>
            _contentType;

        public static bool IsDataContractType(MicroProcessorComponent component, out DataContractType dataContract)
        {
            if (TryGetContentType(component, out var contentType))
            {
                dataContract = new DataContractType(component, contentType);
                return true;
            }
            dataContract = null;
            return false;
        }

        private static bool TryGetContentType(MicroProcessorComponent component, out DataContractContentType contentType)
        {
            if (TryGetDataContractAttribute(component, out var attribute))
            {
                contentType = DataContractContentType.FromAttribute(attribute);
                return true;
            }
            contentType = null;
            return false;
        }

        private static bool TryGetDataContractAttribute(MicroProcessorComponent component, out DataContractAttribute attribute)
        {
            if (component.Type.TryGetAttributeOfType(out attribute))
            {
                if (attribute.Namespace == null)
                {
                    attribute.Namespace = DetermineDefaultContentTypeNamespace(component.Type);
                }
                if (attribute.Name == null)
                {
                    attribute.Name = DetermineDefaultContentTypeName(component.Type);
                }
                return true;
            }
            return false;
        }

        private static string DetermineDefaultContentTypeNamespace(Type type)
        {
            if (TryGetContentTypeNamespaceFromClrNamespace(type, out var contentTypeNamespace))
            {
                return contentTypeNamespace;
            }
            return DataContractContentType.DefaultNamespace;
        }

        private static bool TryGetContentTypeNamespaceFromClrNamespace(Type type, out string contentTypeNamespace)
        {
            foreach (var attribute in GetContractNamespaceAttributesFor(type))
            {
                if (attribute.ClrNamespace == null || attribute.ClrNamespace == type.Namespace)
                {
                    contentTypeNamespace = attribute.ContractNamespace;
                    return true;
                }
            }
            contentTypeNamespace = null;
            return false;
        }

        private static IEnumerable<ContractNamespaceAttribute> GetContractNamespaceAttributesFor(Type type) =>
            from attribute in type.Assembly.GetAttributesOfType<ContractNamespaceAttribute>()
            where attribute.ContractNamespace != null
            orderby attribute.ClrNamespace descending
            select attribute;

        private static string DetermineDefaultContentTypeName(Type type) =>
            type.FriendlyName(true, false);
    }
}
