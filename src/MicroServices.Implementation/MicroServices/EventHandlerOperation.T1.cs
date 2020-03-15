using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class EventHandlerOperation<TEvent> : MessageHandlerRootOperation<TEvent>
    {
        public EventHandlerOperation(MicroProcessor processor, IMessageHandler<TEvent> messageHandler, MessageEnvelope<TEvent> message, CancellationToken? token) :
            this(processor, new HandleAsyncMethod<TEvent>(messageHandler), message, token) { }

        public EventHandlerOperation(MicroProcessor processor, HandleAsyncMethod<TEvent> method, MessageEnvelope<TEvent> message, CancellationToken? token) :
            base(processor, method, message.ToProcess(MessageKind.Event), token) { }

        protected override async Task<MessageHandlerOperationResult> ExecuteAsync(HandleAsyncMethodOperation operation)
        {
            try
            {
                return await base.ExecuteAsync(operation).ConfigureAwait(false);
            }
            catch (BadRequestException exception)
            {
                // When a BadRequestException is thrown while processing an event, it is
                // handled as an error because events are not requests and BadRequestExceptions
                // should not occur in this context.
                throw NewInternalServerErrorException(exception, exception.OperationStackTrace);
            }
        }

        // When a MessageHandlerOperationException was thrown while handling an event, it is regarded as an InternalServerError, because
        // processing an event can never result in a BadRequestException.
        protected override MicroProcessorOperationException NewMicroProcessorOperationException(MessageHandlerOperationException.WithStackTrace exception) =>
            exception.ToInternalServerErrorException(exception.Message);
    }
}
