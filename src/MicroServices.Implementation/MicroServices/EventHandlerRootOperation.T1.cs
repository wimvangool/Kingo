using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class EventHandlerRootOperation<TEvent> : MessageHandlerRootOperation<TEvent>
    {
        public EventHandlerRootOperation(MicroProcessor processor, IMessageHandler<TEvent> messageHandler, Message<TEvent> message, CancellationToken? token) :
            this(processor, new HandleAsyncMethod<TEvent>(messageHandler), message, token) { }

        public EventHandlerRootOperation(MicroProcessor processor, HandleAsyncMethod<TEvent> method, Message<TEvent> message, CancellationToken? token) :
            base(processor, method, message, token) { }

        internal override async Task<MessageHandlerOperationResult<TEvent>> ExecuteAsync(HandleAsyncMethodOperation<TEvent> operation)
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
                throw new InternalServerErrorException(exception.OperationStackTrace, exception.Message, exception);
            }
        }
    }
}
