using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class EventHandlerOperation<TEvent> : MessageHandlerRootOperation<TEvent>
    {
        public EventHandlerOperation(MicroProcessor processor, IMessageHandler<TEvent> messageHandler, Message<TEvent> message, CancellationToken? token) :
            this(processor, new HandleAsyncMethod<TEvent>(messageHandler), message, token) { }

        public EventHandlerOperation(MicroProcessor processor, HandleAsyncMethod<TEvent> method, Message<TEvent> message, CancellationToken? token) :
            base(processor, method, message.ToProcess(MessageKind.Event), token) { }

        public override async Task<MessageHandlerOperationResult> ExecuteAsync()
        {
            try
            {
                return await base.ExecuteAsync().ConfigureAwait(false);
            }
            catch (BadRequestException exception)
            {
                // When a BadRequestException is thrown while processing an event, it is
                // handled as an error because events are not requests and BadRequestExceptions
                // should not occur in this context.
                throw InternalServerErrorException.FromInnerException(exception);
            }
        }

        protected override MicroProcessorOperationException NewMicroProcessorOperationException(MessageHandlerOperationException exception) =>
            exception.AsInternalServerErrorException(exception.Message);
    }
}
