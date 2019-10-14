using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a <see cref="MicroServiceBusController"/> type.
    /// </summary>
    public sealed class MicroServiceBusControllerType : MicroProcessorComponent
    {
        private MicroServiceBusControllerType(MicroProcessorComponent component, params Type[] serviceTypes) :
            base(component, serviceTypes) { }

        private MicroServiceBusControllerType(MicroProcessorComponent component, IEnumerable<Type> serviceTypes) :
            base(component, serviceTypes) { }

        /// <inheritdoc />
        protected override MicroProcessorComponent Copy(IEnumerable<Type> serviceTypes) =>
            new MicroServiceBusControllerType(this, serviceTypes);

        /// <inheritdoc />
        protected override IServiceCollection AddTransientTypeMappingTo(IServiceCollection services) =>
            throw NewLifetimeNotSupportedException();

        /// <inheritdoc />
        protected override IServiceCollection AddScopedTypeMappingTo(IServiceCollection services) =>
            throw NewLifetimeNotSupportedException();

        #region [====== Factory Methods ======]

        internal static bool IsController(Type type, out MicroServiceBusControllerType controller)
        {
            if (IsMicroProcessorComponent(type, out var component))
            {
                return IsController(component, out controller);
            }
            controller = null;
            return false;
        }

        internal static bool IsController(MicroProcessorComponent component, out MicroServiceBusControllerType controller)
        {
            if (typeof(MicroServiceBusController).IsAssignableFrom(component.Type))
            {
                controller = new MicroServiceBusControllerType(component, typeof(MicroServiceBusController), typeof(IHostedService));
                return true;
            }
            controller = null;
            return false;
        }

        #endregion
    }
}
