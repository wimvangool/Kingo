using System;
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

        public HandleMessageMethod(MicroProcessor processor, TMessage message, IMessageHandler<TMessage> handler, CancellationToken? token)           
        {
            _processor = processor;
            _context = new MessageHandlerContext(processor.MessageHandlerFactory, processor.Principal, token, message);
            _message = message;
            _handler = handler;            
        }

        private HandleMessageMethod(MicroProcessor processor, TMessage message, MessageHandlerContext context)         
        {
            _processor = processor;            
            _context = context.CreateContext(message);
            _message = message;
            _handler = null;
        }        

        Task<MessageStream> IMessageHandler.HandleAsync<TEvent>(TEvent message) =>
            CreateMethod(message).InvokeAsync();

        private MicroProcessorMethod<MessageStream> CreateMethod<TEvent>(TEvent message) =>
            new HandleMessageMethod<TEvent>(_processor, message, _context);

        public override async Task<MessageStream> InvokeAsync()
        {
            // If a specific handler to handle the message was specified, that handler is used.
            // If not, then the MessageHandlerFactory is used to instantiate a set of handlers.
            // If the factory cannot find any appropriate handlers, the message is ignored.
            if (_handler == null)
            {
                var stream = MessageStream.Empty;

                foreach (var resolvedHandler in _processor.MessageHandlerFactory.ResolveMessageHandlers(_message, _context))
                {
                    stream = stream.Concat(await InvokeMessageHandlerAsync(resolvedHandler));
                }
                return stream;
            }
            return await InvokeMessageHandlerAsync(new MessageHandlerDecorator<TMessage>(_handler, _message, _context));
        }

        private async Task<MessageStream> InvokeMessageHandlerAsync(MessageHandler handler)
        {
            // Every message handler potentially yields a new stream of events, which is immediately handled by the processor
            // inside the current context. The processor uses a depth-first approach, which means that each event and its resulting
            // sub-tree of events is handled before the next event in the stream.
            var stream = await InvokeMessageHandlerAsyncCore(handler);
            if (stream.Count > 0)
            {
                stream = stream.Concat(await stream.HandleWithAsync(this));
            }
            return stream;
        }

        private async Task<MessageStream> InvokeMessageHandlerAsyncCore(MessageHandler handler)
        {
            using (MicroProcessorContext.CreateScope(_context))
            {
                _context.Token.ThrowIfCancellationRequested();

                try
                {
                    var stream = (await _processor.PipelineFactory.CreatePipeline(handler).Method.InvokeAsync()).GetValue();
                    await _context.UnitOfWork.FlushAsync();
                    return stream;
                }
                catch (MessageHandlerException exception)
                {
                    // Only commands should be converted to BadRequestExceptions if a MessageHandlerException occurs.
                    if (_processor.IsCommand(_message))
                    {
                        throw exception.AsBadRequestException(exception.Message);
                    }
                    throw exception.AsInternalServerErrorException(exception.Message);
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
                    _context.Token.ThrowIfCancellationRequested();
                }                
            }
        }        
    }
}
