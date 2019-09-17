using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class MicroServiceBusComposite : IMicroServiceBus
    {
        private readonly IMicroServiceBus[] _microServiceBusCollection;

        public MicroServiceBusComposite(IEnumerable<IMicroServiceBus> microServiceBusCollection)
        {
            _microServiceBusCollection = microServiceBusCollection.ToArray();
        }

        public Task SendAsync(IEnumerable<IMessageToDispatch> commands) =>
            Task.WhenAll(_microServiceBusCollection.Select(bus => bus.SendAsync(commands)));

        public Task PublishAsync(IEnumerable<IMessageToDispatch> events) =>
            Task.WhenAll(_microServiceBusCollection.Select(bus => bus.PublishAsync(events)));

        public override string ToString() =>
            $"{nameof(IMicroServiceBus)}[{_microServiceBusCollection.Length}]";
    }
}
