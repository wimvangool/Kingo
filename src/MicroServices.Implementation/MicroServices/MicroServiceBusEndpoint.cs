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

        public abstract string Name
        {
            get;
        }

        public abstract Task<IMessageHandlerOperationResult> ProcessAsync(object message, MessageHeader messageHeader, CancellationToken? token = null);

        public override string ToString() =>
            $"{MessageHandler.Type.FriendlyName()}.{MethodInfo.Name}([{MessageKind}] {MessageParameterInfo.ParameterType.FriendlyName()}, ...)";
    }
}
