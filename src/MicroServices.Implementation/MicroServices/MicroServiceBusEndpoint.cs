using System;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal abstract class MicroServiceBusEndpoint : HandleAsyncMethod, IMicroServiceBusEndpoint
    {
        internal MicroServiceBusEndpoint(HandleAsyncMethod method) :
            base(method) { }

        Type IMicroServiceBusEndpoint.MessageHandlerType =>
            MessageHandler.Type;

        public abstract MessageKind MessageKind
        {
            get;
        }

        public abstract Task<IMessageHandlerOperationResult> InvokeAsync(object message, CancellationToken? token = null);

        public override string ToString() =>
            $"{MessageHandler.Type.FriendlyName()}.{Info.Name}([{MessageKind}] {MessageParameterInfo.ParameterType.FriendlyName()}, ...)";
    }
}
