using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class MicroServiceBusStub : IMicroServiceBus
    {
        public Task SendAsync(IEnumerable<IMessageToDispatch> commands) =>
            throw NewNoBusRegisteredException();

        public Task PublishAsync(IEnumerable<IMessageToDispatch> events) =>
            throw NewNoBusRegisteredException();

        public override string ToString() =>
            GetType().FriendlyName();

        private static Exception NewNoBusRegisteredException()
        {
            var messageFormat = ExceptionMessages.MicroServiceBusStub_NoBusRegistered;
            var message = string.Format(messageFormat, typeof(IMicroServiceBus).FriendlyName());
            return new InvalidOperationException(message);
        }
    }
}
