using System.Threading.Tasks;
using Kingo.Messaging.Domain;

namespace Kingo.Messaging
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

            using (var scope = UnitOfWorkContext.StartUnitOfWorkScope(_processor))
            {
                await InvokeAsync(_message);
                await scope.CompleteAsync();
            }
            ThrowIfCancellationRequested();
        }                       

        private async Task InvokeAsync(TMessage message)
        {            
            // If a specific handler to handle the message was specified, that handler is used.
            // If not, then the MessageHandlerFactory is used to instantiate a set of handlers.
            if (_handler == null)
            {
                var messageHandlerFactory = _processor.MessageHandlerFactory;
                if (messageHandlerFactory == null)
                {
                    return;
                }
                // Before the handlers are instantiated, it is checked whether the message comes
                // from an external source, or whether it is an event that was published as a result
                // of processing a previous message. This matters because some handlers may only
                // handle external messages whereas other may only handle internal messages.
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
                // When a DomainException is thrown, a BusinessRule or some other
                // Constraint-By-Design was broken. That means that if the message
                // is a Command, the Exception is promoted to a FunctionalException,
                // which will communicate an intentional (pre-condition) failure as
                // opposed to a technical (post-condition) failure.
                if (_processor.IsCommand(message))
                {
                    throw exception.AsCommandExecutionException(message);
                }
                throw;
            }
            ThrowIfCancellationRequested();
        }                       
    }
}
