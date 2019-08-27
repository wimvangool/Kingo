using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class MicroServiceBusComposite : IMicroServiceBus
    {
        private readonly IMicroServiceBus[] _microServiceBuses;

        public MicroServiceBusComposite(IEnumerable<IMicroServiceBus> microServiceBuses)
        {
            _microServiceBuses = microServiceBuses.ToArray();
        }

        public Task PublishAsync(IEnumerable<object> messages) =>
            Task.WhenAll(_microServiceBuses.Select(bus => bus.PublishAsync(messages)));

        public Task PublishAsync(object message) =>
            Task.WhenAll(_microServiceBuses.Select(bus => bus.PublishAsync(message)));

        public override string ToString() =>
            string.Join(" + ", _microServiceBuses.Select(bus => bus.GetType().FriendlyName()));
    }
}
