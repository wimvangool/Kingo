using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kingo.Clocks;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{   
    internal sealed class MessageBus : IMessageBus
    {
        private readonly MessageFactory _messageFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly List<Message<object>> _messages;
        private readonly IClock _clock;

        public MessageBus(MessageFactory messageFactory, IServiceProvider serviceProvider, IClock clock)
        {
            _messageFactory = messageFactory;
            _serviceProvider = serviceProvider;
            _messages = new List<Message<object>>();
            _clock = clock;
        }

        #region [====== IReadOnlyList<IMessage> ======]

        public int Count =>
            _messages.Count;

        public IMessage this[int index] =>
            _messages[index];

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public IEnumerator<IMessage> GetEnumerator() =>
            _messages.GetEnumerator();

        /// <inheritdoc />
        public override string ToString() =>
            ToString(_messages.Count(IsCommand), _messages.Count(IsEvent));

        private static string ToString(int commandCount, int eventCount) =>
            $"{commandCount} command(s), {eventCount} event(s)";

        private static bool IsCommand(IMessage message) =>
            message.Kind == MessageKind.Command;

        private static bool IsEvent(IMessage message) =>
            message.Kind == MessageKind.Event;

        #endregion

        #region [====== Send & Publish ======]

        public void Send(object command, TimeSpan delay) =>
            Send(command, ToDeliveryTime(delay));

        public void Send(object command, DateTimeOffset? deliveryTime = null) =>
            Add(IsNotNull(command, nameof(command)), MessageKind.Command, deliveryTime);

        public void Publish(object @event, TimeSpan delay) =>
            Publish(@event, ToDeliveryTime(delay));

        public void Publish(object @event, DateTimeOffset? deliveryTime = null) =>
            Add(IsNotNull(@event, nameof(@event)), MessageKind.Event, deliveryTime);

        private void Add(object content, MessageKind kind, DateTimeOffset? deliveryTime) =>
            _messages.Add(_messageFactory.CreateMessage(kind, MessageDirection.Output, MessageHeader.Unspecified, content, deliveryTime).Validate(_serviceProvider));

        private DateTimeOffset ToDeliveryTime(TimeSpan delay)
        {
            if (delay < TimeSpan.Zero)
            {
                throw NewNegativeDelayNotAllowedException(delay);
            }
            return _clock.UtcDateAndTime().Add(delay);
        }

        private static Exception NewNegativeDelayNotAllowedException(in TimeSpan delay)
        {
            var messageFormat = ExceptionMessages.MessageBus_NegativeDelayNotAllowed;
            var message = string.Format(messageFormat, delay);
            return new ArgumentOutOfRangeException(nameof(delay), message);
        }

        #endregion

        #region [====== ToResult ======]

        public MessageBusResult ToResult() =>
            new MessageBusResult(_messages);

        #endregion
    }
}
