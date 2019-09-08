using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.MicroServices
{   
    internal sealed class MessageBus : IMessageBus
    {
        private readonly List<MessageToDispatch> _messages;
      
        public MessageBus()
        {
            _messages = new List<MessageToDispatch>(); 
        }

        #region [====== IReadOnlyList<MessageToDispatch> ======]

        public int Count =>
            _messages.Count;

        public MessageToDispatch this[int index] =>
            _messages[index];

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public IEnumerator<MessageToDispatch> GetEnumerator() =>
            _messages.GetEnumerator();

        #endregion

        /// <inheritdoc />
        public void Send(object command, DateTimeOffset? deliveryTime = null) =>
            _messages.Add(MessageToDispatch.CreateCommand(command, deliveryTime));

        /// <inheritdoc />
        public void Publish(object @event, DateTimeOffset? deliveryTime) =>
            _messages.Add(MessageToDispatch.CreateEvent(@event, deliveryTime));

        /// <inheritdoc />
        public override string ToString() =>
            ToString(this);

        internal static string ToString(IReadOnlyList<MessageToDispatch> messages) =>
            ToString(messages.Count(IsCommand), messages.Count(IsEvent));

        private static string ToString(int commandCount, int eventCount) =>
            $"{commandCount} command(s), {eventCount} event(s)";

        private static bool IsCommand(MessageToDispatch message) =>
            message.Kind == MessageKind.Command;

        private static bool IsEvent(MessageToDispatch message) =>
            message.Kind == MessageKind.Event;
    }
}
