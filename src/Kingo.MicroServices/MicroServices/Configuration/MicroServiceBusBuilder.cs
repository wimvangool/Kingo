using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Configuration
{
    public sealed class MicroServiceBusBuilder : IServiceCollectionConfigurator
    {
        private readonly List<IMicroServiceBus> _serviceBusCollection;

        internal MicroServiceBusBuilder()
        {
            _serviceBusCollection = new List<IMicroServiceBus>();
        }

        void IServiceCollectionConfigurator.Configure(IServiceCollection services) =>
            services.AddSingleton(CreateServiceBus());

        public void Add(IMicroServiceBus serviceBus) =>
            _serviceBusCollection.Add(serviceBus ?? throw new ArgumentNullException(nameof(serviceBus)));

        private IMicroServiceBus CreateServiceBus()
        {
            if (_serviceBusCollection.Count == 0)
            {
                return MicroServiceBus.Null;
            }
            if (_serviceBusCollection.Count == 1)
            {
                return _serviceBusCollection[0];
            }
            return new MicroServiceBusComposite(_serviceBusCollection);
        }
    }
}
