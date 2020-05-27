using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices
{
    internal sealed class MessageFactory : IMessageKindResolver, IMessageIdGenerator
    {
        private readonly Dictionary<MessageDirection, MessageValidationOptions> _messageValidationOptions;
        private readonly IMessageKindResolver _messageKindResolver;
        private readonly IMessageIdGenerator _messageIdGenerator;

        public MessageFactory(IDictionary<MessageDirection, MessageValidationOptions> messageValidationOptions, IMessageKindResolver messageKindResolver, IMessageIdGenerator messageIdGenerator)
        {
            _messageValidationOptions = new Dictionary<MessageDirection, MessageValidationOptions>(messageValidationOptions);
            _messageKindResolver = messageKindResolver;
            _messageIdGenerator = messageIdGenerator;
        }

        public Message<TContent> CreateCommand<TContent>(MessageDirection direction, MessageHeader header, TContent content, DateTimeOffset? deliveryTime = null) =>
            CreateMessage(MessageKind.Command, direction, header, content, deliveryTime);

        public Message<TContent> CreateEvent<TContent>(MessageDirection direction, MessageHeader header, TContent content, DateTimeOffset? deliveryTime = null) =>
            CreateMessage(MessageKind.Event, direction, header, content, deliveryTime);

        public Message<TContent> CreateRequest<TContent>(MessageDirection direction, MessageHeader header, TContent content, DateTimeOffset? deliveryTime = null) =>
            CreateMessage(MessageKind.Request, direction, header, content, deliveryTime);

        public Message<TContent> CreateResponse<TContent>(MessageDirection direction, MessageHeader header, TContent content, DateTimeOffset? deliveryTime = null) =>
            CreateMessage(MessageKind.Response, direction, header, content, deliveryTime);

        public Message<TContent> CreateMessage<TContent>(IMessage<TContent> message) =>
            CreateMessage(message.Kind, message.Direction, new MessageHeader(message.MessageId).WithCorrelationId(message.CorrelationId), message.Content, message.DeliveryTimeUtc);

        public Message<TContent> CreateMessage<TContent>(MessageKind kind, MessageDirection direction, MessageHeader header, TContent content, DateTimeOffset? deliveryTime = null) =>
            CreateMessageValidator(IsValid(kind), IsValid(direction), header, content).DeliverAt(deliveryTime).ConvertTo<TContent>();

        private Message<object> CreateMessageValidator(MessageKind kind, MessageDirection direction, MessageHeader header, object content)
        {
            var message = CreateMessageImplementation(direction, header, content);
            var messageValidationOptions = ResolveValidationOptions(direction);
            return new MessageValidator<object>(message, kind, messageValidationOptions);
        }

        private Message<object> CreateMessageImplementation(MessageDirection direction, MessageHeader header, object content)
        {
            var messageKind = _messageKindResolver.ResolveMessageKind(content);
            var messageHeader = header.WithMessageId(this, content);
            return new MessageImplementation<object>(messageKind, direction, messageHeader, content);
        }

        public MessageKind ResolveMessageKind(Type contentType) =>
            _messageKindResolver.ResolveMessageKind(contentType);

        public string GenerateMessageId(object content) =>
            _messageIdGenerator.GenerateMessageId(content);

        private MessageValidationOptions ResolveValidationOptions(MessageDirection direction)
        {
            if (_messageValidationOptions.TryGetValue(direction, out var validationOptions))
            {
                return validationOptions;
            }
            return MessageValidationOptions.None;
        }

        private static MessageKind IsValid(MessageKind kind)
        {
            if (IsEqualToAny(kind, MessageKind.Command, MessageKind.Event, MessageKind.Request, MessageKind.Response))
            {
                return kind;
            }
            throw NewInvalidKindException(kind);
        }

        private static MessageDirection IsValid(MessageDirection direction)
        {
            if (IsEqualToAny(direction, MessageDirection.Input, MessageDirection.Internal, MessageDirection.Output))
            {
                return direction;
            }
            throw NewInvalidDirectionException(direction);
        }

        private static bool IsEqualToAny<TValue>(TValue value, params TValue[] values) where TValue : struct =>
            values.Any(expectedValue => expectedValue.Equals(value));

        private static Exception NewInvalidKindException(MessageKind kind)
        {
            var messageFormat = ExceptionMessages.MessageFactory_InvalidMessageKind;
            var message = string.Format(messageFormat, kind);
            return new ArgumentOutOfRangeException(nameof(kind), message);
        }

        private static Exception NewInvalidDirectionException(MessageDirection direction)
        {
            var messageFormat = ExceptionMessages.MessageFactory_InvalidMessageDirection;
            var message = string.Format(messageFormat, direction);
            return new ArgumentOutOfRangeException(nameof(direction), message);
        }
    }
}
