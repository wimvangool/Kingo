using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class MicroServiceBusStub : IMicroServiceBus
    {
        public Task SendAsync(IEnumerable<IMessageToDispatch> commands) =>
            Task.CompletedTask;

        public Task PublishAsync(IEnumerable<IMessageToDispatch> events) =>
            Task.CompletedTask;

        public override string ToString() =>
            GetType().FriendlyName();
    }
}
