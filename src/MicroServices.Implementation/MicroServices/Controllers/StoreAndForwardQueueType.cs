using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class StoreAndForwardQueueType : MicroProcessorComponent
    {
        private readonly IMicroServiceBusResolver _queueTypeFactory;

        private StoreAndForwardQueueType(MicroProcessorComponent component, params Type[] serviceTypes) :
            this(component, serviceTypes, null) { }

        private StoreAndForwardQueueType(MicroProcessorComponent component, Type[] serviceTypes, IMicroServiceBusResolver queueTypeFactory) :
            base(component, serviceTypes)
        {
            _queueTypeFactory = queueTypeFactory;
        }

        public StoreAndForwardQueueType AssignQueueTypeFactory(IMicroServiceBusResolver queueTypeFactory) =>
            new StoreAndForwardQueueType(this, ServiceTypes.ToArray(), queueTypeFactory);

        protected override MicroProcessorComponent Copy(IEnumerable<Type> serviceTypes) =>
            new StoreAndForwardQueueType(this, serviceTypes.ToArray(), _queueTypeFactory);

        #region [====== AddTo ======]

        protected override IServiceCollection AddTransientTypeMappingTo(IServiceCollection services) =>
            throw NewLifetimeNotSupportedException();

        protected override IServiceCollection AddScopedTypeMappingTo(IServiceCollection services) =>
            throw NewLifetimeNotSupportedException();

        protected override IServiceCollection AddSingletonTypeMappingTo(IServiceCollection services)
        {
            if (_queueTypeFactory == null)
            {
                return base.AddSingletonTypeMappingTo(services);
            }
            return services.AddSingleton(Type, _queueTypeFactory.ResolveMicroServiceBus);
        }

        #endregion

        internal static bool IsStoreAndForwardQueue(Type type, out StoreAndForwardQueueType queueType)
        {
            if (IsMicroProcessorComponent(type, out var component))
            {
                if (typeof(StoreAndForwardQueue).IsAssignableFrom(component.Type))
                {
                    queueType = new StoreAndForwardQueueType(component, typeof(StoreAndForwardQueue), typeof(IHostedService));
                    return true;
                }
            }
            queueType = null;
            return false;
        }
    }
}
