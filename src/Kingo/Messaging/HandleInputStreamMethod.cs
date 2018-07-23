using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a method that handles a stream of messages and returns all events that were published.
    /// </summary>
    public sealed class HandleInputStreamMethod : MicroProcessorMethod<IMessageStream>, IMessageHandler
    {
        #region [====== HandleMessageMethod ======]

        private sealed class HandleMessageMethod<TMessage> : HandleMessageMethod
        {
            private readonly HandleInputStreamMethod _inputStreamMethod;
            private readonly TMessage _message;
            private readonly IMessageHandler<TMessage> _handler;

            public HandleMessageMethod(HandleInputStreamMethod inputStreamMethod, TMessage message, IMessageHandler<TMessage> handler)
            {
                _inputStreamMethod = inputStreamMethod;
                _message = message;
                _handler = handler;
            }

            private MicroProcessor Processor =>
                _inputStreamMethod._processor;

            public override MicroProcessorContext Context =>
                _inputStreamMethod._context;

            public override async Task InvokeAsync()
            {
                // If a specific handler to handle the message was specified, that handler is used.
                // If not, then the MessageHandlerFactory is used to instantiate a set of handlers.
                // If the factory cannot find any appropriate handlers, the message is ignored.
                if (_handler == null)
                {
                    foreach (var resolvedHandler in Processor.MessageHandlerFactory.ResolveMessageHandlers(Context.OperationType, _message))
                    {
                        await InvokeMessageHandlerAsync(resolvedHandler);
                    }
                }
                else
                {
                    await InvokeMessageHandlerAsync(new MessageHandlerDecorator<TMessage>(_handler, _message));
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
                    await outputStream.HandleMessagesWithAsync(_inputStreamMethod);
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
                    _inputStreamMethod._context.Reset();
                }
            }
        }

        #endregion

        private readonly MicroProcessor _processor;
        private readonly MessageHandlerContext _context;
        private readonly IMessageStream _inputStream;
        private IMessageStream _outputStream;

        internal HandleInputStreamMethod(MicroProcessor processor, CancellationToken? token, IMessageStream inputStream)
        {
            _processor = processor;
            _context = new MessageHandlerContext(processor.Principal, token);
            _inputStream = inputStream;
            _outputStream = MessageStream.Empty;
        }

        /// <inheritdoc />
        public override CancellationToken Token =>
            _context.Token;

        /// <summary>
        /// The input-stream that will be handled by this method.
        /// </summary>
        public IMessageStream InputStream =>
            _inputStream;

        /// <summary>
        /// Handles the input-stream that was provided to the processor and returns all published events.
        /// </summary>
        /// <returns>All events that were published while handling the input-stream.</returns>
        /// <exception cref="ExternalProcessorException">
        /// Something went wrong while handling the input-stream.
        /// </exception>
        public override async Task<IMessageStream> InvokeAsync()
        {
            using (MicroProcessorContext.CreateScope(_context))
            {                
                await _inputStream.HandleMessagesWithAsync(this);                
            }
            return _outputStream;
        }

        async Task IMessageHandler.HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler)
        {
            _context.Token.ThrowIfCancellationRequested();

            try
            {
                await HandleAsync(message, handler, _context.StackTrace.IsEmpty ? MicroProcessorOperationTypes.InputStream : MicroProcessorOperationTypes.OutputStream);
            }
            finally
            {
                _context.Token.ThrowIfCancellationRequested();
            }
        }            

        private async Task HandleAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler, MicroProcessorOperationTypes operationType)
        {            
            _context.StackTraceCore.Push(operationType, message);

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
                _context.StackTraceCore.Pop();
            }
        }

        private async Task HandleInputMessageAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler)
        {
            try
            {
                await HandleMessageAsync(message, handler);
                await _context.UnitOfWorkCore.FlushAsync();
            }
            catch (InternalProcessorException exception)
            {
                // Only commands should be converted to BadRequestExceptions if an InternalProcessorException occurs.
                if (_processor.IsCommand(message))
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
                _context.Token.ThrowIfCancellationRequested();
            }
        }       

        private async Task HandleOutputMessageAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler)
        {
            _outputStream = _outputStream.Append(message);

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
                _context.Token.ThrowIfCancellationRequested();
            }
        }            

        private Task HandleMessageAsync<TMessage>(TMessage message, IMessageHandler<TMessage> handler) =>
            _processor.HandleMessageAsync(new HandleMessageMethod<TMessage>(this, message, handler));                     
    }
}
