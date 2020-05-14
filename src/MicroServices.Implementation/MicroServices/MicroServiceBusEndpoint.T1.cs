using System;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    internal sealed class MicroServiceBusEndpoint<TMessage> : MicroServiceBusEndpoint
    {
        #region [====== MessageHandlerOperationFactory ======]

        private abstract class MessageHandlerOperationFactory
        {
            protected abstract MicroServiceBusEndpoint<TMessage> Endpoint
            {
                get;
            }

            public abstract MessageKind MessageKind
            {
                get;
            }

            public MessageHandlerOperation<TMessage> CreateOperation(Message<TMessage> message, CancellationToken? token) =>
                CreateOperation(message, token, CreateMethod());

            protected abstract MessageHandlerOperation<TMessage> CreateOperation(Message<TMessage> message, CancellationToken? token, HandleAsyncMethod<TMessage> method);

            private HandleAsyncMethod<TMessage> CreateMethod() =>
                new HandleAsyncMethod<TMessage>(new MessageHandlerResolver(Endpoint._processor, Endpoint.MessageHandler), Endpoint);
        }

        #endregion

        #region [====== CommandHandlerOperationFactory ======]

        private sealed class CommandHandlerOperationFactory : MessageHandlerOperationFactory
        {
            private readonly MicroServiceBusEndpoint<TMessage> _endpoint;

            public CommandHandlerOperationFactory(MicroServiceBusEndpoint<TMessage> endpoint)
            {
                _endpoint = endpoint;
            }

            public override MessageKind MessageKind =>
                MessageKind.Command;

            protected override MicroServiceBusEndpoint<TMessage> Endpoint =>
                _endpoint;

            protected override MessageHandlerOperation<TMessage> CreateOperation(Message<TMessage> message, CancellationToken? token, HandleAsyncMethod<TMessage> method) =>
                new CommandHandlerOperation<TMessage>(Endpoint._processor, method, message, token);
        }

        #endregion

        #region [====== EventHandlerOperationFactory ======]

        private sealed class EventHandlerOperationFactory : MessageHandlerOperationFactory
        {
            private readonly MicroServiceBusEndpoint<TMessage> _endpoint;

            public EventHandlerOperationFactory(MicroServiceBusEndpoint<TMessage> endpoint)
            {
                _endpoint = endpoint;
            }

            public override MessageKind MessageKind =>
                MessageKind.Event;

            protected override MicroServiceBusEndpoint<TMessage> Endpoint =>
                _endpoint;

            protected override MessageHandlerOperation<TMessage> CreateOperation(Message<TMessage> message, CancellationToken? token, HandleAsyncMethod<TMessage> method) =>
                new EventHandlerOperation<TMessage>(Endpoint._processor, method, message, token);
        }

        #endregion

        #region [====== MessageHandlerResolver ======]

        private sealed class MessageHandlerResolver : IMessageHandler<TMessage>
        {
            private readonly MicroProcessor _processor;
            private readonly MessageHandlerComponent _messageHandler;

            public MessageHandlerResolver(MicroProcessor processor, MessageHandlerComponent messageHandler)
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
        private readonly MessageHandlerOperationFactory _operationFactory;
        private readonly MessageBusEndpointAttribute _attribute;

        public MicroServiceBusEndpoint(HandleAsyncMethod method, MicroProcessor processor, MessageBusEndpointAttribute attribute) : base(method)
        {            
            _processor = processor;
            _operationFactory = CreateOperationFactory(processor.MessageFactory.ResolveMessageKind(typeof(TMessage)));
            _attribute = attribute;
        }

        public override string Name =>
            _attribute.NameFormat.FormatName(_processor.Settings.ServiceName, MessageHandler.Type, MessageParameterInfo.ParameterType);

        public override MessageKind MessageKind =>
            _operationFactory.MessageKind;

        #region [====== ProcessAsync ======]

        public override async Task<MessageHandlerOperationResult<object>> ProcessAsync(object message, MessageHeader messageHeader, CancellationToken? token = null)
        {
            var messageToProcess = _processor.MessageFactory.CreateMessage(MessageKind, MessageDirection.Input, messageHeader, IsNotNull(message, nameof(message)));
            if (messageToProcess.TryConvertTo<TMessage>(out var messageOfSupportedType))
            {
                return (await ProcessAsync(messageOfSupportedType, token).ConfigureAwait(false)).ConvertTo<object>();
            }
            throw NewMessageNotOfSupportedTypeException(Name, typeof(TMessage), message);
        }

        private async Task<MessageHandlerOperationResult<TMessage>> ProcessAsync(Message<TMessage> message, CancellationToken? token)
        {
            // We create a new scope here because endpoints are typically hosted in an environment where
            // the infrastructure does not create a scope upon receiving a new message.
            using (_processor.ServiceProvider.CreateScope())
            {
                return await _processor.ExecuteWriteOperationAsync(_operationFactory.CreateOperation(message, token)).ConfigureAwait(false);
            }
        }

        private static Exception NewMessageNotOfSupportedTypeException(string endpointName, Type endpointMessageType, object message)
        {
            var errorMessageFormat = ExceptionMessages.MicroServiceBusEndpoint_MessageNotOfSupportedType;
            var errorMessage = string.Format(errorMessageFormat, message.GetType().FriendlyName(), endpointName, endpointMessageType.FriendlyName());
            return new ArgumentException(errorMessage, nameof(message));
        }

        #endregion

        #region [====== CreateOperationFactory ======]

        private MessageHandlerOperationFactory CreateOperationFactory(MessageKind messageKind)
        {
            switch (messageKind)
            {
                case MessageKind.Command:
                    return new CommandHandlerOperationFactory(this);
                case MessageKind.Event:
                    return new EventHandlerOperationFactory(this);
            }
            throw NewInvalidMessageKindSpecifiedException(messageKind);
        }

        private static Exception NewInvalidMessageKindSpecifiedException(MessageKind messageKind)
        {
            var messageFormat = ExceptionMessages.MicroProcessorEndpoint_UnuspportedMessageKind;
            var message = string.Format(messageFormat, messageKind, typeof(TMessage).FriendlyName(), nameof(MessageAttribute), nameof(MessageFactory));
            return new InvalidOperationException(message);
        }

        #endregion
    }
}
