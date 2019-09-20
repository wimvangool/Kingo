using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class MicroServiceBusControllerType : MicroProcessorComponent
    {
        private MicroServiceBusControllerType(MicroProcessorComponent component, IEnumerable<Type> serviceTypes) :
            base(component, serviceTypes) { }

        protected override MicroProcessorComponent Copy(IEnumerable<Type> serviceTypes) =>
            new MicroServiceBusControllerType(this, serviceTypes);

        protected override IServiceCollection AddTransientTypeMappingTo(IServiceCollection services) =>
            throw NewLifetimeNotSupportedException();

        protected override IServiceCollection AddScopedTypeMappingTo(IServiceCollection services) =>
            throw NewLifetimeNotSupportedException();

        internal static MicroServiceBusControllerType FromComponent(MicroProcessorComponent component, bool isMainController)
        {
            if (typeof(MicroServiceBusController).IsAssignableFrom(component.Type))
            {
                return new MicroServiceBusControllerType(component, DetermineServiceTypes(isMainController));
            }
            return null;
        }       

        private static IEnumerable<Type> DetermineServiceTypes(bool isMainController)
        {
            yield return typeof(MicroServiceBusController);
            yield return typeof(IHostedService);

            if (isMainController)
            {
                yield return typeof(IMicroServiceBus);
            }
        }
    }
}
