using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.BuildingBlocks.ComponentModel.Server.Domain;

namespace Kingo.BuildingBlocks.ComponentModel.Server
{
    internal sealed class MessageHandlerDispatcher<TMessage> : MessageHandlerDispatcher where TMessage : class, IMessage<TMessage>
    {       
        private readonly TMessage _message;
        private readonly IMessageHandler<TMessage> _handler;
        private readonly MessageProcessor _processor;        

        internal MessageHandlerDispatcher(TMessage message, IMessageHandler<TMessage> handler, MessageProcessor processor)
        {
            _message = message;
            _handler = handler;
            _processor = processor;            
        }

        public override IMessage Message
        {
            get { return _message; }
        }

        public async override Task InvokeAsync()
        {
            ThrowIfCancellationRequested();

            using (var scope = _processor.CreateUnitOfWorkScope())
            {
                await InvokeAsync(_message);
                await scope.CompleteAsync();
            }
            ThrowIfCancellationRequested();
        }                       

        private async Task InvokeAsync(TMessage message)
        {
            if (_handler == null)
            {
                var messageHandlerFactory = _processor.MessageHandlerFactory;
                if (messageHandlerFactory == null)
                {
                    return;
                }
                var source = MessageProcessor.CurrentMessage.DetermineMessageSourceOf(message);

                foreach (var handler in messageHandlerFactory.CreateMessageHandlersFor(message, source))
                {
                    await InvokeAsync(message, handler);
                }
            }
            else
            {
                await InvokeAsync(message, new MessageHandlerInstance<TMessage>(_handler));
            }            
        }

        private async Task InvokeAsync(TMessage message, MessageHandlerInstance<TMessage> handler)
        {
            var messageHandler = new MessageHandlerWrapper<TMessage>(message, handler);            
            var pipeline = _processor.BuildCommandOrEventHandlerPipeline().ConnectTo(messageHandler);

            try
            {
                await pipeline.InvokeAsync();
            }            
            catch (DomainException exception)
            {
                FunctionalException functionalException;

                if (TryConvertToFunctionalException(messageHandler, exception, out functionalException))
                {
                    throw functionalException;
                }
                throw;
            }
            ThrowIfCancellationRequested();
        }     
        
        private static bool TryConvertToFunctionalException(IMessageHandler handler, DomainException exception, out FunctionalException functionalException)
        {
            foreach (var exceptionFilter in GetDomainExceptionFilters(handler))
            {
                if (exceptionFilter.TryConvertToFunctionalException(handler.Message, exception, out functionalException))
                {
                    return true;
                }
            }
            functionalException = null;
            return false;
        }

        private static IEnumerable<IDomainExceptionFilter> GetDomainExceptionFilters(IMessageHandler handler)
        {
            var classAttributes = handler.GetClassAttributesOfType<IDomainExceptionFilter>();
            var methodAttributes = handler.GetMethodAttributesOfType<IDomainExceptionFilter>();

            return classAttributes.Concat(methodAttributes);
        }
    }
}
