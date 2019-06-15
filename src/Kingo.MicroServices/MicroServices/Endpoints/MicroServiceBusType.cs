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

        private MicroServiceBusType(MicroProcessorComponent component, params Type[] serviceTypes) :
            base(component)
        {
            _serviceTypes = serviceTypes;
        }

        public IEnumerable<Type> ServiceTypes =>
            _serviceTypes;

        public static IEnumerable<MicroServiceBusType> FromComponents(IEnumerable<MicroProcessorComponent> components)
        {
            foreach (var component in components)
            {
                if (IsMicroServiceBusType(component, out var microServiceBus))
                {
                    yield return microServiceBus;
                }
            }
        }

        public static bool IsMicroServiceBusType(Type type, out MicroServiceBusType microServiceBus)
        {
            if (IsMicroProcessorComponent(type, out var component))
            {
                return IsMicroServiceBusType(component, out microServiceBus);
            }
            microServiceBus = null;
            return false;
        }

        private static bool IsMicroServiceBusType(MicroProcessorComponent component, out MicroServiceBusType microServiceBus)
        {            
            if (component.Type.ImplementsInterface<IMicroServiceBus>())
            {
                var hostedServiceInterface = component.Type.GetInterfacesOfType<IHostedService>().FirstOrDefault();
                if (hostedServiceInterface == null)
                {
                    microServiceBus = new MicroServiceBusType(component);
                }
                else
                {
                    microServiceBus = new MicroServiceBusType(component, hostedServiceInterface);
                }
                return true;
            }
            microServiceBus = null;
            return false;
        }
    }
}
