using System.Threading;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal abstract class MicroServiceBusEndpoint : HandleAsyncMethod, IMicroServiceBusEndpoint
    {
        internal MicroServiceBusEndpoint(HandleAsyncMethod method) :
            base(method) { }

        ITypeAttributeProvider IMicroServiceBusEndpoint.MessageHandler =>
            MessageHandler;

        public abstract MessageKind MessageKind
        {
            get;
        }

        public abstract Task<IMessageHandlerOperationResult> InvokeAsync(object message, CancellationToken? token = null);

        public override string ToString() =>
            $"{MessageHandler.Type.FriendlyName()}.{Info.Name}([{MessageKind}] {MessageParameter.Type.FriendlyName()}, ...)";
    }
}
