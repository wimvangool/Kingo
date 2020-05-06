using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kingo.Clocks;

namespace Kingo.MicroServices
{   
    internal sealed class MessageBus : IMessageBus
    {
        private readonly IMessageFactory _messageFactory;
        private readonly List<IMessage> _messages;
        private readonly IClock _clock;

        public MessageBus(IMessageFactory messageFactory, IClock clock)
        {
            _messageFactory = messageFactory;
            _messages = new List<IMessage>();
            _clock = clock;
        }

        #region [====== IReadOnlyList<MessageToDispatch> ======]

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

        #region [====== Send ======]

        public void Send(object command, TimeSpan delay)
        {
            throw new NotImplementedException();
        }

        public void Send(object command, DateTimeOffset? deliveryTime = null) =>
            //_messages.Add(_messageFactory.Wrap(command).ToDispatch(MessageKind.Command, deliveryTime));
            throw new NotImplementedException();

        #endregion

        #region [====== Publish ======]

        public void Publish(object @event, TimeSpan delay)
        {
            throw new NotImplementedException();
        }

        public void Publish(object @event, DateTimeOffset? deliveryTime = null) =>
            //_messages.Add(_messageFactory.Wrap(@event).ToDispatch(MessageKind.Event, deliveryTime));
            throw new NotImplementedException();

        #endregion
    }
}
