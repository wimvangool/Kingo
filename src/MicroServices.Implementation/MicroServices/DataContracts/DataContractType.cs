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
            if (component.Type.TryGetAttributeOfType(out DataContractAttribute attribute))
            {
                contentType = DataContractContentType.FromType(component.Type, attribute);
                return true;
            }
            contentType = null;
            return false;
        }
    }
}
