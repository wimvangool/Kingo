using System;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;

namespace Kingo.Messaging
{
    internal sealed class HandleInputStreamAsyncMethod : MicroProcessorMethod<MessageHandlerContext>, IMessageHandler
    {
        public static async Task<IMessageStream> Invoke(MicroProcessor processor, IMessageStream inputStream, CancellationToken? token)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }
            if (inputStream.Count == 0)
            {
                return MessageStream.Empty;
            }
            try
            {
                return await Invoke(processor, inputStream, new MessageHandlerContext(processor.Principal, token));
            }
            catch (ConcurrencyException exception)
            {
                if (inputStream.Count == 1 && processor.IsCommand(inputStream[0]))
                {
                    throw exception.AsBadRequestException(exception.Message);
                }
                throw exception.AsInternalServerErrorException(exception.Message);
            }
            catch (InternalProcessorException exception)
            {
                throw exception.AsInternalServerErrorException(exception.Message);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (ExternalProcessorException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw InternalServerErrorException.FromInnerException(exception);
            }            
        }

        private static async Task<IMessageStream> Invoke(MicroProcessor processor, IMessageStream inputStream, MessageHandlerContext context)
        {
            using (var scope = MicroProcessorContext.CreateScope(context))
            {
                var method = new HandleInputStreamAsyncMethod(processor, context);

                await inputStream.HandleMessagesWithAsync(method);
                await scope.CompleteAsync();
               
                return method.OutputStream;
            }
        }

        private HandleInputStreamAsyncMethod(MicroProcessor processor, MessageHandlerContext context)
        {
            Processor = processor;
            Context = context;            
            OutputStream = MessageStream.Empty;
        }

        protected override MicroProcessor Processor
        {
            get;
        }

        protected override MessageHandlerContext Context
        {
            get;
        }

        private IMessageStream OutputStream
        {
            get;
            set;
        }

        public async Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler)
        {
            Context.Token.ThrowIfCancellationRequested();
            Context.StackTraceCore.Push(CreateMessageInfo(message));

            // Every event that is published to the context's OutputStream is added to the final output stream.
            if (Context.StackTraceCore.CurrentSource == MessageSources.OutputStream)
            {
                OutputStream = OutputStream.Append(message);
            }
            try
            {
                await HandleAsyncCore(message, handler);
            }            
            catch (InternalProcessorException exception)
            {
                // Only commands should be converted to BadRequestExceptions if an InternalProcessorException occurs.
                if (IsCommand(message))
                {
                    throw exception.AsBadRequestException(exception.Message);
                }
                throw exception.AsInternalServerErrorException(exception.Message);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (ExternalProcessorException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw InternalServerErrorException.FromInnerException(exception);
            }
            finally
            {
                Context.StackTraceCore.Pop();
                Context.Token.ThrowIfCancellationRequested();
            }
        }

        private MessageInfo CreateMessageInfo(object message) =>
            Context.StackTraceCore.IsEmpty ? MessageInfo.FromInputStream(message) : MessageInfo.FromOutputStream(message);

        private async Task HandleAsyncCore<TMessage>(TMessage message, IMessageHandler<TMessage> handler)
        {
            // If a specific handler to handle the message was specified, that handler is used.
            // If not, then the MessageHandlerFactory is used to instantiate a set of handlers.
            // If the factory cannot find any appropriate handlers, the message is ignored.
            if (handler == null)
            {
                foreach (var resolvedHandler in Processor.MessageHandlerFactory.ResolveMessageHandlers(Context.StackTraceCore.CurrentSource, message))
                {
                    await InvokeMessageHandlerAsync(resolvedHandler);
                }
            }
            else
            {
                await InvokeMessageHandlerAsync(new MessageHandlerDecorator<TMessage>(handler, message));
            }
        }

        private async Task InvokeMessageHandlerAsync(MessageHandler handler)
        {
            // Every message handler potentially yields a new stream of events, which is immediately handled by the processor
            // inside the current context. The processor uses a depth-first approach, which means that each event and its resulting
            // sub-tree of events is handled before the next event in the stream.
            var outputStream = await InvokeMessageHandlerAsyncCore(handler);
            if (outputStream.Count > 0)
            {
                await outputStream.HandleMessagesWithAsync(this);
            }
        }            
        
        private async Task<IMessageStream> InvokeMessageHandlerAsyncCore(MessageHandler handler)
        {
            try
            {
                return (await Processor.Pipeline.Build(handler).InvokeAsync(Context)).Value;
            }
            finally
            {
                Context.Reset();
            }    
        }

        private bool IsCommand(object message) =>
            Context.StackTraceCore.CurrentSource == MessageSources.InputStream && Processor.IsCommand(message);        
    }
}
