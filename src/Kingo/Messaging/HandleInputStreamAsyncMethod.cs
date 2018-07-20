using System;
using System.Threading;
using System.Threading.Tasks;

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
            return await Invoke(processor, inputStream, new MessageHandlerContext(processor.Principal, token));                      
        }

        private static async Task<IMessageStream> Invoke(MicroProcessor processor, IMessageStream inputStream, MessageHandlerContext context)
        {
            using (MicroProcessorContext.CreateScope(context))
            {
                var method = new HandleInputStreamAsyncMethod(processor, context);
                await inputStream.HandleMessagesWithAsync(method);                               
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

        public Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler) =>
            HandleAsync(message, handler, Context.StackTrace.IsEmpty ? MicroProcessorOperationTypes.InputStream : MicroProcessorOperationTypes.OutputStream);

        private async Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler, MicroProcessorOperationTypes operationType)
        {
            Context.Token.ThrowIfCancellationRequested();
            Context.StackTraceCore.Push(operationType, message);

            try
            {                
                if (operationType == MicroProcessorOperationTypes.InputStream)
                {
                    await HandleInputMessageAsync(message, handler);
                }
                else if (operationType == MicroProcessorOperationTypes.OutputStream)
                {
                    await HandleOutputMessageAsync(message, handler);
                }
            }
            finally
            {
                Context.StackTraceCore.Pop();
            }
        }

        private async Task HandleInputMessageAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler)
        {
            try
            {
                await HandleMessageAsync(message, handler);
                await Context.UnitOfWorkCore.FlushAsync();
            }
            catch (InternalProcessorException exception)
            {
                // Only commands should be converted to BadRequestExceptions if an InternalProcessorException occurs.
                if (Processor.IsCommand(message))
                {
                    throw exception.AsBadRequestException(exception.Message);
                }
                throw exception.AsInternalServerErrorException(exception.Message);
            }            
            catch (ExternalProcessorException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw InternalServerErrorException.FromInnerException(exception);
            }
            finally
            {
                Context.Token.ThrowIfCancellationRequested();
            }
        }       

        private async Task HandleOutputMessageAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler)
        {
            OutputStream = OutputStream.Append(message);

            try
            {
                await HandleMessageAsync(message, handler);
            }
            catch (InternalProcessorException exception)
            {
                throw exception.AsInternalServerErrorException(exception.Message);
            }
            finally
            {
                Context.Token.ThrowIfCancellationRequested();
            }
        }            

        private async Task HandleMessageAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler)
        {
            // If a specific handler to handle the message was specified, that handler is used.
            // If not, then the MessageHandlerFactory is used to instantiate a set of handlers.
            // If the factory cannot find any appropriate handlers, the message is ignored.
            if (handler == null)
            {
                foreach (var resolvedHandler in Processor.MessageHandlerFactory.ResolveMessageHandlers(Context.OperationType, message))
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
    }
}
