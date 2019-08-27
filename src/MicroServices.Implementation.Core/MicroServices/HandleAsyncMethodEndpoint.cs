using System.Threading;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal abstract class HandleAsyncMethodEndpoint : HandleAsyncMethod, IHandleAsyncMethodEndpoint
    {
        internal HandleAsyncMethodEndpoint(HandleAsyncMethod method) :
            base(method) { }

        ITypeAttributeProvider IHandleAsyncMethodEndpoint.MessageHandler =>
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
