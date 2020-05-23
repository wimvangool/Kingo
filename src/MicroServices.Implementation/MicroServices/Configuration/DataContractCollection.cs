using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.MicroServices.DataContracts;
using Kingo.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Configuration
{
    /// <summary>
    /// Represents a collection of data-contract types that are registered with a <see cref="IMicroProcessor"/>
    /// so that it can use one or more <see cref="IDataContractSerializer"/>-instances to serialize and
    /// deserialize data-contracts.
    /// </summary>
    public sealed class DataContractCollection : MicroProcessorComponentCollection
    {
        private readonly Dictionary<Type, DataContractType> _dataContractTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContractCollection" /> class.
        /// </summary>
        public DataContractCollection()
        {
            _dataContractTypes = new Dictionary<Type, DataContractType>();
        }

        #region [====== IMicroProcessorComponentCollection ======]

        /// <inheritdoc />
        public override IEnumerator<MicroProcessorComponent> GetEnumerator() =>
            Enumerable.Empty<MicroProcessorComponent>().GetEnumerator();

        /// <inheritdoc />
        protected override IServiceCollection AddSpecificComponentsTo(IServiceCollection services) =>
            AddDataContractSerializerFactoryTo(services, BuildDataContractTypeMap());

        private static IServiceCollection AddDataContractSerializerFactoryTo(IServiceCollection services, DataContractTypeMap dataContractTypeMap) =>
            services.AddTransient(provider => BuildDataContractSerializerFactory(provider, dataContractTypeMap));

        private static IDataContractSerializerFactory BuildDataContractSerializerFactory(IServiceProvider serviceProvider, DataContractTypeMap dataContractTypeMap)
        {
            var serializerFactory = serviceProvider.GetService<ISerializerFactory>();
            if (serializerFactory == null)
            {
                serializerFactory = new SerializerFactory(new SerializerTypeMap(), serviceProvider);
            }
            return new DataContractSerializerFactory(serializerFactory, dataContractTypeMap);
        }

        private DataContractTypeMap BuildDataContractTypeMap()
        {
            var dataContractTypeMap = new DataContractTypeMap();

            foreach (var dataContractType in _dataContractTypes.Values)
            {
                dataContractTypeMap.Add(dataContractType);
            }
            return dataContractTypeMap;
        }

        #endregion

        #region [====== Add ======]

        /// <inheritdoc />
        protected override bool Add(MicroProcessorComponent component)
        {
            if (DataContractType.IsDataContractType(component, out var dataContract))
            {
                _dataContractTypes[dataContract.Type] = dataContract;
                return true;
            }
            return false;
        }

        #endregion
    }
}
