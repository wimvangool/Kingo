using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Endpoints
{
    internal sealed class MicroServiceBusControllerType : MicroProcessorComponent
    {
        private readonly Type[] _serviceTypes;

        private MicroServiceBusControllerType(MicroProcessorComponent component, params Type[] serviceTypes) :
            base(component)
        {
            _serviceTypes = serviceTypes;
        }

        public IEnumerable<Type> ServiceTypes =>
            _serviceTypes;

        public static IEnumerable<MicroServiceBusControllerType> FromComponents(IEnumerable<MicroProcessorComponent> components)
        {
            foreach (var component in components)
            {
                if (IsMicroProcessorBusControllerType(component, out var controller))
                {
                    yield return controller;
                }
            }
        }

        public static bool IsMicroProcessorBusControllerType(Type type, out MicroServiceBusControllerType controller)
        {
            if (IsMicroProcessorComponent(type, out var component))
            {
                return IsMicroProcessorBusControllerType(component, out controller);
            }
            controller = null;
            return false;
        }

        private static bool IsMicroProcessorBusControllerType(MicroProcessorComponent component, out MicroServiceBusControllerType controller)
        {
            if (typeof(MicroServiceBusController).IsAssignableFrom(component.Type))
            {
                controller = new MicroServiceBusControllerType(component, typeof(IHostedService));
                return true;
            }
            controller = null;
            return false;
        }
    }
}
