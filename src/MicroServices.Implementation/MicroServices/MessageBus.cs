using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{   
    internal sealed class MessageBus : IMessageBus
    {
        private readonly MessageEnvelopeFactory _messageFactory;
        private readonly List<MessageToDispatch> _messages;
      
        public MessageBus(MessageEnvelopeFactory messageFactory)
        {
            _messageFactory = messageFactory;
            _messages = new List<MessageToDispatch>(); 
        }

        public void SendCommand(object command, DateTimeOffset? deliveryTime = null) =>
            _messages.Add(_messageFactory.Wrap(command).ToDispatch(MessageKind.Command, deliveryTime));

        public void PublishEvent(object @event, DateTimeOffset? deliveryTime = null) =>
            _messages.Add(_messageFactory.Wrap(@event).ToDispatch(MessageKind.Event, deliveryTime));

        #region [====== IReadOnlyList<MessageToDispatch> ======]

        public int Count =>
            _messages.Count;

        public MessageToDispatch this[int index] =>
            _messages[index];

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public IEnumerator<MessageToDispatch> GetEnumerator() =>
            _messages.GetEnumerator();

        /// <inheritdoc />
        public override string ToString() =>
            ToString(_messages.Count(IsCommand), _messages.Count(IsEvent));

        private static string ToString(int commandCount, int eventCount) =>
            $"{commandCount} command(s), {eventCount} event(s)";

        private static bool IsCommand(MessageToDispatch message) =>
            message.Kind == MessageKind.Command;

        private static bool IsEvent(MessageToDispatch message) =>
            message.Kind == MessageKind.Event;

        #endregion
    }
}
