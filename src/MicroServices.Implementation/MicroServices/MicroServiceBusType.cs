using System;

namespace Kingo.MicroServices
{
    internal sealed class MicroServiceBusType : MicroProcessorComponent
    {
        private MicroServiceBusType(MicroProcessorComponent component, params Type[] serviceTypes) :
            base(component, serviceTypes) { }

        public static MicroServiceBusType FromComponent(MicroProcessorComponent component)
        {
            if (typeof(IMicroServiceBus).IsAssignableFrom(component.Type))
            {
                return new MicroServiceBusType(component, typeof(IMicroServiceBus));
            }
            return null;
        }
    }
}
