using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.DataContracts
{
    internal sealed class DataContractTypeMap
    {
        private readonly Dictionary<Type, DataContractType> _dataContractTypes;
        private readonly Dictionary<DataContractType, Type> _clrTypes;

        public DataContractTypeMap()
        {
            _dataContractTypes = new Dictionary<Type, DataContractType>();
            _clrTypes = new Dictionary<DataContractType, Type>();
        }

        private DataContractTypeMap(DataContractTypeMap map)
        {
            _dataContractTypes = new Dictionary<Type, DataContractType>(map._dataContractTypes);
            _clrTypes = new Dictionary<DataContractType, Type>(map._clrTypes);
        }

        public DataContractTypeMap Copy() =>
            new DataContractTypeMap(this);

        public override string ToString() =>
            $"{_dataContractTypes.Count} mapping(s) registered";

        public DataContractType GetContentType(Type clrType)
        {
            throw new NotImplementedException();
        }

        public Type GetClrType(DataContractType contentType)
        {
            throw new NotImplementedException();
        }

        public void Add(Type clrType, DataContractType contentType)
        {
            throw new NotImplementedException();
        }
    }
}
