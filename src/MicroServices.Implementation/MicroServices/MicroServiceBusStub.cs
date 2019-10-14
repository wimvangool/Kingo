using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class MicroServiceBusStub : IMicroServiceBus
    {
        public Task SendCommandsAsync(IEnumerable<IMessageToDispatch> commands) =>
            Task.CompletedTask;

        public Task PublishEventsAsync(IEnumerable<IMessageToDispatch> events) =>
            Task.CompletedTask;

        public override string ToString() =>
            GetType().FriendlyName();
    }
}
