using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices.Endpoints
{
    internal sealed class MicroServiceBusComposite : MicroServiceBus
    {
        private readonly IMicroServiceBus[] _microServiceBuses;

        public MicroServiceBusComposite(IEnumerable<IMicroServiceBus> microServiceBuses)
        {
            _microServiceBuses = microServiceBuses.ToArray();
        }

        public override Task PublishAsync(IEnumerable<IMessage> messages) =>
            Task.WhenAll(_microServiceBuses.Select(bus => bus.PublishAsync(messages)));

        public override Task PublishAsync(IMessage message) =>
            Task.WhenAll(_microServiceBuses.Select(bus => bus.PublishAsync(message)));

        public override string ToString() =>
            string.Join(" + ", _microServiceBuses.Select(bus => bus.GetType().FriendlyName()));
    }
}
