using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{
    internal sealed class MicroServiceBusEndpoint<TMessage> : MicroServiceBusEndpoint
    {
        #region [====== MessageHandlerResolver ======]

        private sealed class MessageHandlerResolver : IMessageHandler<TMessage>
        {
            private readonly MicroProcessor _processor;
            private readonly MessageHandler _messageHandler;

            public MessageHandlerResolver(MicroProcessor processor, MessageHandler messageHandler)
            {
                _processor = processor;
                _messageHandler = messageHandler;
            }

            public Task HandleAsync(TMessage message, MessageHandlerOperationContext context) =>
                ResolveMessageHandler().HandleAsync(message, context);

            private IMessageHandler<TMessage> ResolveMessageHandler() =>
                _processor.ServiceProvider.GetRequiredService(_messageHandler.Type) as IMessageHandler<TMessage>;
        }

        #endregion        

        private readonly MicroProcessor _processor;
        private readonly bool _isCommandEndpoint;

        public MicroServiceBusEndpoint(HandleAsyncMethod method, MicroProcessor processor, EndpointAttribute attribute) :
            base(method)
        {            
            _processor = processor;
            _isCommandEndpoint = attribute.IsCommandEndpoint(processor.Options.Endpoints.MessageKindResolver, typeof(TMessage));
        }        

        public override MessageKind MessageKind =>
            _isCommandEndpoint ? MessageKind.Command : MessageKind.Event;

        public override Task<IMessageHandlerOperationResult> InvokeAsync(object message, CancellationToken? token = null)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (message is TMessage messageOfSupportedType)
            {
                return InvokeAsync(messageOfSupportedType, token);
            }
            return Task.FromResult<IMessageHandlerOperationResult>(MessageBufferResult.Empty);
        }

        private async Task<IMessageHandlerOperationResult> InvokeAsync(TMessage message, CancellationToken? token)
        {
            // We create a new scope here because endpoints are typically hosted in an environment where
            // the infrastructure does not create a scope upon receiving a new message (like in ASP.NET).
            using (_processor.ServiceProvider.CreateScope())
            {
                return await _processor.ExecuteOperationAsync(CreateOperation(message, token));
            }
        }

        private MessageHandlerOperation<TMessage> CreateOperation(TMessage message, CancellationToken? token) =>
            CreateOperation(message, token, CreateMethod());

        private MessageHandlerOperation<TMessage> CreateOperation(TMessage message, CancellationToken? token, HandleAsyncMethod<TMessage> method)
        {
            if (_isCommandEndpoint)
            {
                return new CommandHandlerOperation<TMessage>(_processor, method, message, token);
            }
            return new EventHandlerOperation<TMessage>(_processor, method, message, token);            
        }        

        private HandleAsyncMethod<TMessage> CreateMethod() =>
            new HandleAsyncMethod<TMessage>(new MessageHandlerResolver(_processor, MessageHandler), this);        
    }
}
