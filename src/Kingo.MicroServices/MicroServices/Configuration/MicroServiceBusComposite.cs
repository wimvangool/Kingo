using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices.Configuration
{
    internal sealed class MicroServiceBusComposite : IMicroServiceBus
    {
        private readonly IMicroServiceBus[] _serviceBusCollection;

        public MicroServiceBusComposite(IEnumerable<IMicroServiceBus> serviceBusCollection)
        {
            _serviceBusCollection = serviceBusCollection.ToArray();
        }

        public override string ToString() =>
            string.Join("+", _serviceBusCollection.Select(bus => bus.GetType().FriendlyName()));

        public Task PublishAsync(object message) =>
            Task.WhenAll(_serviceBusCollection.Select(bus => bus.PublishAsync(message)));

        public Task PublishAsync(IEnumerable<object> messages) =>
            PublishAsync(messages.ToArray());

        private Task PublishAsync(object[] messages) =>
            Task.WhenAll(_serviceBusCollection.Select(bus => bus.PublishAsync(messages)));
    }
}
