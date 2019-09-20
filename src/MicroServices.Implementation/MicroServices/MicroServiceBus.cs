using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a basic implementation of the <see cref="IMicroServiceBus" /> interface.
    /// </summary>
    public sealed class MicroServiceBus : IMicroServiceBus
    {
        private readonly Lazy<IMicroServiceBus> _bus;

        internal MicroServiceBus(IServiceProvider serviceProvider, IEnumerable<Type> serviceBusTypes)
        {
            _bus = new Lazy<IMicroServiceBus>(() => CreateServiceBus(serviceProvider, serviceBusTypes), true);
        }

        private IMicroServiceBus Bus
        {
            get
            {
                try
                {
                    return _bus.Value;
                }
                catch (InvalidOperationException exception)
                {
                    throw NewCircularDependencyDetectedException(exception);
                }
            }
        }

        /// <inheritdoc />
        public Task SendAsync(IEnumerable<IMessageToDispatch> commands) =>
            Bus.SendAsync(commands);

        /// <inheritdoc />
        public Task PublishAsync(IEnumerable<IMessageToDispatch> events) =>
            Bus.PublishAsync(events);

        internal static IMicroServiceBus ResolveMicroServiceBus(MicroProcessor processor) =>
            CreateServiceBus(processor.ServiceProvider.GetServices<IMicroServiceBus>().ToArray());

        private static IMicroServiceBus CreateServiceBus(IServiceProvider serviceProvider, IEnumerable<Type> serviceBusTypes) =>
            CreateServiceBus(serviceBusTypes.Select(serviceProvider.GetRequiredService).OfType<IMicroServiceBus>().ToArray());

        private static IMicroServiceBus CreateServiceBus(IMicroServiceBus[] serviceBusCollection)
        {
            if (serviceBusCollection.Length == 0)
            {
                return new MicroServiceBusStub();
            }
            if (serviceBusCollection.Length == 1)
            {
                return serviceBusCollection[0];
            }
            return new MicroServiceBusComposite(serviceBusCollection);
        }

        private static Exception NewCircularDependencyDetectedException(Exception innerException)
        {
            var messageFormat = ExceptionMessages.MicroServiceBus_CircularDependencyDetected;
            var message = string.Format(messageFormat, typeof(IMicroServiceBus).FriendlyName());
            return new InvalidOperationException(message, innerException);
        }
    }
}
