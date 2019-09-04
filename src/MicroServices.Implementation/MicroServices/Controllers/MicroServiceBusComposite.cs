using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class MicroServiceBusComposite : IMicroServiceBus
    {
        private readonly IMicroServiceBus[] _microServiceBusCollection;

        public MicroServiceBusComposite(IMicroServiceBus[] microServiceBusCollection)
        {
            _microServiceBusCollection = microServiceBusCollection.ToArray();
        }

        public Task PublishAsync(IEnumerable<object> events) =>
            Task.WhenAll(_microServiceBusCollection.Select(bus => bus.PublishAsync(events)));

        public Task PublishAsync(object @event) =>
            Task.WhenAll(_microServiceBusCollection.Select(bus => bus.PublishAsync(@event)));

        public override string ToString() =>
            $"{nameof(IMicroServiceBus)}[{_microServiceBusCollection.Length}]";
    }
}
