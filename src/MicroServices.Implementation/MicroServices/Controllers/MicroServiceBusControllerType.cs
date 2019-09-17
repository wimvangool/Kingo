using System;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class MicroServiceBusControllerType : MicroProcessorComponent
    {
        private MicroServiceBusControllerType(MicroProcessorComponent component, params Type[] serviceTypes) :
            base(component, serviceTypes) { }                       
        
        internal static MicroServiceBusControllerType FromComponent(MicroProcessorComponent component)
        {
            if (typeof(MicroServiceBusController).IsAssignableFrom(component.Type))
            {
                return new MicroServiceBusControllerType(component, typeof(MicroServiceBusController), typeof(IHostedService));                
            }
            return null;
        }       
    }
}
