using System;
using System.Collections.Generic;
using Kingo.Reflection;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class MicroServiceBusType : MicroProcessorComponent
    {
        private MicroServiceBusType(MicroProcessorComponent component, params Type[] serviceTypes) :
            base(component, serviceTypes) { }          

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
                microServiceBus = new MicroServiceBusType(component);
                return true;
            }
            microServiceBus = null;
            return false;
        }
    }
}
