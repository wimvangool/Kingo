using System;
using System.Collections;
using System.Collections.Generic;

namespace Kingo.MicroServices
{   
    internal sealed class EventBus : IEventBus
    {
        #region [====== Message ======]

        private sealed class Message : MessageType, IMessage
        {
            private readonly object _instance;

            private Message(object instance) :
                base(instance.GetType(), MessageKind.Event)
            {
                _instance = instance;
            }

            public object Instance =>
                _instance;

            public static Message FromInstance(object message) =>
                new Message(message ?? throw new ArgumentNullException(nameof(message)));
        }

        #endregion

        private readonly List<Message> _messages;
      
        public EventBus()
        {
            _messages = new List<Message>();
        }

        #region [====== IReadOnlyList<object> ======]

        public int Count =>
            _messages.Count;

        public IMessage this[int index] =>
            _messages[index];

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public IEnumerator<IMessage> GetEnumerator() =>
            _messages.GetEnumerator();

        #endregion

        /// <inheritdoc />
        public void Publish(object message) =>
            _messages.Add(Message.FromInstance(message));

        /// <inheritdoc />
        public override string ToString() =>
            ToString(this);
        
        internal static string ToString(IReadOnlyList<object> events) =>
            $"{events.Count} event(s) published";
    }
}
