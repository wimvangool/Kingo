﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class HandleMessageMethod<TMessage> : MicroProcessorMethod<MessageStream>, IMessageHandler
    {        
        private readonly MicroProcessor _processor;
        private readonly MessageHandlerContext _context;
        private readonly TMessage _message;
        private readonly IMessageHandler<TMessage> _handler;
        private readonly bool _isRootMethod;

        public HandleMessageMethod(MicroProcessor processor, TMessage message, IMessageHandler<TMessage> handler, CancellationToken? token)           
        {
            _processor = processor;
            _context = new MessageHandlerContext(processor.ServiceProvider, processor.Principal, token, message);
            _message = message;
            _handler = handler;
            _isRootMethod = true;            
        }

        private HandleMessageMethod(MicroProcessor processor, TMessage message, MessageHandlerContext context)         
        {
            _processor = processor;            
            _context = context.CreateContext(message);
            _message = message;
            _handler = null;
            _isRootMethod = false;
        }        

        public int InvocationCount
        {
            get;
            private set;
        }

        Task<MessageStream> IMessageHandler.HandleAsync<TEvent>(TEvent message)
        {
            var method = new HandleMessageMethod<TEvent>(_processor, message, _context);

            try
            {
                return method.InvokeAsync();
            }
            finally
            {
                InvocationCount += method.InvocationCount;
            }
        }            

        public override async Task<MessageStream> InvokeAsync()
        {
            var scope = MicroProcessorContext.CreateScope(_context);

            try
            {
                var stream = await HandleMessageAsync().ConfigureAwait(false);

                if (_isRootMethod)
                {
                    await _context.UnitOfWork.FlushAsync().ConfigureAwait(false);
                }
                return stream;
            }
            catch (MessageHandlerException exception)
            {
                // Only commands should be converted to BadRequestExceptions if a MessageHandlerException occurs.
                if (_isRootMethod && _processor.IsCommand(_message))
                {
                    throw exception.AsBadRequestException(exception.Message);
                }
                throw exception.AsInternalServerErrorException(exception.Message);
            }
            catch (BadRequestException exception)
            {
                // If a BadRequestException if thrown explicitly by user code or in the pipeline,
                // then convert it to an internal server error if the root message is an event.
                if (_isRootMethod && !_processor.IsCommand(_message))
                {
                    throw InternalServerErrorException.FromInnerException(exception);
                }
                throw;
            }
            catch (MicroProcessorException)
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
                scope.Dispose();
            }
        }

        private async Task<MessageStream> HandleMessageAsync()
        {
            // If a specific handler to handle the message was specified, that handler is used.
            // If not, then the MessageHandlerFactory is used to instantiate a set of handlers.
            // If the factory cannot find any appropriate handlers, the message is ignored.
            if (_handler == null)
            {
                var stream = MessageStream.Empty;

                foreach (var resolvedHandler in _processor.MessageHandlers.ResolveMessageHandlers(_message, _context))
                {
                    stream = stream.Concat(await InvokeMessageHandlerAsync(resolvedHandler).ConfigureAwait(false));
                }
                return stream;
            }
            return await InvokeMessageHandlerAsync(new MessageHandlerDecorator<TMessage>(_handler, _message, _context)).ConfigureAwait(false);
        }

        private async Task<MessageStream> InvokeMessageHandlerAsync(MessageHandler handler)
        {
            InvocationCount++;

            // Every message handler potentially yields a new stream of events, which is immediately handled by the processor
            // inside the current context. The processor uses a depth-first approach, which means that each event and its resulting
            // sub-tree of events is handled before the next event in the stream.
            var stream = await InvokeMessageHandlerAsyncCore(handler).ConfigureAwait(false);
            if (stream.Count > 0)
            {
                stream = stream.Concat(await stream.HandleWithAsync(this).ConfigureAwait(false));
            }
            return stream;
        }

        private async Task<MessageStream> InvokeMessageHandlerAsyncCore(MessageHandler handler)
        {
            _context.Token.ThrowIfCancellationRequested();

            try
            {
                return (await _processor.Pipeline.CreatePipeline(handler).Method.InvokeAsync().ConfigureAwait(false)).GetValue();
            }
            finally
            {
                _context.Token.ThrowIfCancellationRequested();
            }            
        }        
    }
}
