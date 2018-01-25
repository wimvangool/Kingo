using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging
{
    internal abstract class HandleStreamAsyncMethod : MicroProcessorMethod<MessageHandlerContext>, IMessageHandler
    {        
        public async Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler)
        {
            Context.Token.ThrowIfCancellationRequested();
            Context.Messages.Push(CreateMessageInfo(message));
            
            try
            {
                await HandleAsyncCore(message, handler);
            }
            finally
            {
                Context.Messages.Pop();
                Context.Token.ThrowIfCancellationRequested();
            }
        }

        protected abstract MessageInfo CreateMessageInfo(object message);

        protected virtual async Task HandleAsyncCore<TMessage>(TMessage message, IMessageHandler<TMessage> handler)
        {
            // If a specific handler to handle the message was specified, that handler is used.
            // If not, then the MessageHandlerFactory is used to instantiate a set of handlers.
            // If no factory is defined, or the factory cannot find any appropriate handlers,
            // the message is ignored.
            if (handler == null)
            {
                foreach (var resolvedHandler in Processor.MessageHandlerFactory.ResolveMessageHandlers(Context, message))
                {
                    await InvokeMessageHandler(message, resolvedHandler);
                }
            }
            else
            {
                await InvokeMessageHandler(message, new MessageHandlerDecorator<TMessage>(Context, handler));
            }
        }

        private async Task InvokeMessageHandler<TMessage>(TMessage message, MessageHandler<TMessage> handler)
        {
            // Every message potentially returns a new stream of events, which is immediately handled by the processor
            // inside the current context. The processor uses a depth-first approach, which means that each event and its resulting
            // sub-tree of events is handled before the next event in the stream.
            var pipeline = Processor.Pipeline.Build(handler);
            var result = Context.CreateHandleAsyncResult();

            try
            {
                result = await pipeline.HandleAsync(message, Context);
                result.Commit();                
            }
            catch
            {
                // If an exception is thrown, the metadata events must still be handled.
                result = Context.CreateHandleAsyncResult().RemoveOutputStream();
                result.Commit();
                throw;
            }
            finally
            {                
                await HandleStreamsAsync(result.MetadataStream, result.OutputStream);
            }            
        }        

        protected abstract Task HandleStreamsAsync(IMessageStream metadataStream, IMessageStream outputStream);

        protected static InternalServerErrorException NewEventHandlerException(object @event, InternalProcessorException exception)
        {
            var messageFormat = ExceptionMessages.HandleStreamAsyncMethod_EventHandlerException;
            var message = string.Format(messageFormat, @event.GetType().FriendlyName());
            return exception.AsInternalServerErrorException(@event, message);
        }
    }
}
