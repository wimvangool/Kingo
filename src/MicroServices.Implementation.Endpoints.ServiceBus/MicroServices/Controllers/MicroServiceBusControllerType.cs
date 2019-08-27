using System;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class MicroServiceBusControllerType : MicroProcessorComponent
    {
        private MicroServiceBusControllerType(MicroProcessorComponent component, params Type[] serviceTypes) :
            base(component, serviceTypes) { }                       
        
        internal static MicroServiceBusControllerType FromComponent(MicroProcessorComponent component, bool isMainController)
        {
            if (typeof(MicroServiceBusController).IsAssignableFrom(component.Type))
            {
                if (isMainController)
                {
                    return new MicroServiceBusControllerType(component, typeof(IHostedService), typeof(IMicroServiceBus));
                }
                return new MicroServiceBusControllerType(component, typeof(IHostedService));                
            }
            return null;
        }       
    }
}
