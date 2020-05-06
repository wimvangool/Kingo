using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    internal sealed class MessageFactory : IMessageFactory
    {
        private readonly MessageValidationOptions _messageValidationOptions;
        private readonly IMessageKindResolver _messageKindResolver;
        private readonly IMessageIdGenerator _messageIdGenerator;

        public MessageFactory(MessageValidationOptions messageValidationOptions, IMessageKindResolver messageKindResolver, IMessageIdGenerator messageIdGenerator)
        {
            _messageValidationOptions = messageValidationOptions;
            _messageKindResolver = messageKindResolver;
            _messageIdGenerator = messageIdGenerator;
        }

        public IMessage CreateMessage(object content, DateTimeOffset? deliveryTime = null) =>
            _MessageFactoryDelegates.GetOrAdd(IsNotNull(content, nameof(content)).GetType(), CreateMessageFactoryDelegate).Invoke(this, content, deliveryTime);

        public Message<TContent> CreateMessage<TContent>(TContent content, DateTimeOffset? deliveryTime = null)
        {
            var messageKind = _messageKindResolver.ResolveMessageKind(content);
            var messageId = _messageIdGenerator.GenerateMessageId(content);
            var message = new Message<TContent>(messageKind, messageId, content);

            return deliveryTime.HasValue ? message.DeliverAt(deliveryTime.Value) : message;
        }

        public MessageKind ResolveMessageKind(Type contentType) =>
            _messageKindResolver.ResolveMessageKind(contentType);

        public string GenerateMessageId(object content) =>
            _messageIdGenerator.GenerateMessageId(content);

        #region [====== MessageFactoryDelegates ======]

        private static readonly ConcurrentDictionary<Type, Func<MessageFactory, object, DateTimeOffset?, IMessage>> _MessageFactoryDelegates =
            new ConcurrentDictionary<Type, Func<MessageFactory, object, DateTimeOffset?, IMessage>>();

        private static Func<MessageFactory, object, DateTimeOffset?, IMessage> CreateMessageFactoryDelegate(Type contentType)
        {
            var messageFactoryParameter = Expression.Parameter(typeof(MessageFactory), "messageFactory");
            var contentParameter = Expression.Parameter(typeof(object), "content");
            var deliveryTimeParameter = Expression.Parameter(typeof(DateTimeOffset?), "deliveryTime");

            var contentExpression = Expression.Convert(contentParameter, contentType);
            var createMessageMethod = CreateMessageMethodDefinition.MakeGenericMethod(contentType);
            var createMessageMethodInvocation = Expression.Call(messageFactoryParameter, createMessageMethod, contentExpression, deliveryTimeParameter);

            return CreateLambdaExpression(createMessageMethodInvocation, messageFactoryParameter, contentParameter, deliveryTimeParameter).Compile();
        }

        private static Expression<Func<MessageFactory, object, DateTimeOffset?, IMessage>> CreateLambdaExpression(Expression body, params ParameterExpression[] parameters) =>
            Expression.Lambda<Func<MessageFactory, object, DateTimeOffset?, IMessage>>(body, parameters);

        #endregion

        #region [====== CreateMessageMethodDefinition ======]

        private static readonly Lazy<MethodInfo> _CreateMessageMethodDefinition = new Lazy<MethodInfo>(GetCreateMessageMethodDefinition, true);

        private static MethodInfo CreateMessageMethodDefinition =>
            _CreateMessageMethodDefinition.Value;

        private static MethodInfo GetCreateMessageMethodDefinition()
        {
            var methods =
                from method in typeof(MessageFactory).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                where method.Name == nameof(CreateMessage) && method.IsGenericMethodDefinition
                select method;

            return methods.Single();
        }

        #endregion
    }
}
