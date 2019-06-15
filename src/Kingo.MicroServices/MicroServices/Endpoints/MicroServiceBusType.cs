using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Reflection;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Endpoints
{
    internal sealed class MicroServiceBusType : MicroProcessorComponent
    {
        private readonly Type[] _serviceTypes;

        private MicroServiceBusType(Type type, params Type[] serviceTypes) :
            base(type)
        {
            _serviceTypes = serviceTypes;
        }

        public IEnumerable<Type> ServiceTypes =>
            _serviceTypes;

        public static bool IsMicroServiceBusType(Type type, out MicroServiceBusType microServiceBus)
        {
            if (CanBeCreatedFrom(type) && type.ImplementsInterface<IMicroServiceBus>())
            {
                var hostedServiceInterface = type.GetInterfacesOfType<IHostedService>().FirstOrDefault();
                if (hostedServiceInterface == null)
                {
                    microServiceBus = new MicroServiceBusType(type);
                }
                else
                {
                    microServiceBus = new MicroServiceBusType(type, hostedServiceInterface);
                }
                return true;
            }
            microServiceBus = null;
            return false;
        }
    }
}
