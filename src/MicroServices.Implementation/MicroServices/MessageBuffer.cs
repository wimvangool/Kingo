using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Kingo.Collections.Generic;
using Kingo.Reflection;

namespace Kingo.MicroServices
{    
    internal sealed class MessageBuffer : IMessageBuffer, IReadOnlyList<MessageToDispatch>
    {
        #region [====== BufferedMessage ======]

        private abstract class BufferedMessage : IMessageBuffer
        {
            public abstract MessageToDispatch Message
            {
                get;
            }

            public abstract Task<MessageHandlerOperationResult> HandleEventsWith(IMessageProcessor processor, MessageHandlerOperationContext context);

            public override string ToString() =>
                Message.ToString();
        }

        #endregion

        #region [====== BufferedMessage<TMessage> ======]

        private sealed class BufferedMessage<TMessage> : BufferedMessage
        {
            public BufferedMessage(MessageToDispatch message)
            {
                Message = message;
            }

            public override MessageToDispatch Message
            {
                get;
            }

            public override Task<MessageHandlerOperationResult> HandleEventsWith(IMessageProcessor processor, MessageHandlerOperationContext context)
            {
                // Only events that are not scheduled for the future can be handled within the same logical operation/transaction.
                if (IsUnscheduledEvent(Message))
                {
                    return processor.HandleAsync(new MessageToProcess<TMessage>((TMessage) Message.Content, Message.Kind), context);
                }
                return Task.FromResult<MessageHandlerOperationResult>(MessageBufferResult.Empty);
            }

            private static bool IsUnscheduledEvent(MessageToDispatch message) =>
                message.Kind == MessageKind.Event && message.DeliveryTimeUtc == null;
        }

        #endregion

        public static readonly MessageBuffer Empty = new MessageBuffer(Enumerable.Empty<MessageToDispatch>());

        private readonly BufferedMessage[] _messages;

        public MessageBuffer(IEnumerable<MessageToDispatch> messages) :
            this(messages.WhereNotNull().Select(CreateBufferedMessage)) { }

        private MessageBuffer(IEnumerable<BufferedMessage> messages)
        {
            _messages = messages.ToArray();
        }

        internal MessageBuffer Append(MessageBuffer buffer) =>
            new MessageBuffer(_messages.Concat(buffer._messages));

        #region [====== IReadOnlyList<MessageToDispatch> ======]

        public int Count =>
            _messages.Length;

        public MessageToDispatch this[int index] =>
            _messages[index].Message;

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public IEnumerator<MessageToDispatch> GetEnumerator() =>
            _messages.AsEnumerable().Select(message => message.Message).GetEnumerator();

        public override string ToString() =>
            MessageBus.ToString(this);

        #endregion

        #region [====== IMessageBuffer ======]

        public async Task<MessageHandlerOperationResult> HandleEventsWith(IMessageProcessor processor, MessageHandlerOperationContext context)
        {
            var result = MessageBufferResult.Empty;

            foreach (var message in _messages)
            {
                result = result.Append(await message.HandleEventsWith(processor, context).ConfigureAwait(false));
            }
            return result;
        }

        #endregion

        #region [====== CreateBufferedMessage ======]

        private static readonly ConcurrentDictionary<Type, Func<object, BufferedMessage>> _MessageConstructors = new ConcurrentDictionary<Type, Func<object, BufferedMessage>>();

        private static BufferedMessage CreateBufferedMessage(MessageToDispatch message) =>
            GetOrAddBufferedMessageConstructorFor(message.Content.GetType()).Invoke(message);

        private static Func<object, BufferedMessage> GetOrAddBufferedMessageConstructorFor(Type messageType) =>
            _MessageConstructors.GetOrAdd(messageType, GetBufferedMessageConstructorFor);

        private static Func<object, BufferedMessage> GetBufferedMessageConstructorFor(Type messageType)
        {            
            var bufferedMessageType = typeof(BufferedMessage<>).MakeGenericType(messageType);
            var constructor = bufferedMessageType.GetConstructor(new[] { messageType });
            var messageParameter = Expression.Parameter(typeof(object), "message");
            var message = Expression.Convert(messageParameter, messageType);
            var body = Expression.New(constructor, message);

            return Expression.Lambda<Func<object, BufferedMessage>>(body, messageParameter).Compile();
        }

        #endregion
    }
}
