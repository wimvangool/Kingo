using System;
using System.Collections;
using System.Collections.Generic;

namespace Kingo.MicroServices
{   
    internal sealed class EventBus : IEventBus
    {
        private readonly List<object> _messages;
      
        public EventBus()
        {
            _messages = new List<object>();
        }

        #region [====== IReadOnlyList<object> ======]

        public int Count =>
            _messages.Count;

        public object this[int index] =>
            _messages[index];

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public IEnumerator<object> GetEnumerator() =>
            _messages.GetEnumerator();

        #endregion

        /// <inheritdoc />
        public void Publish(object message) =>
            _messages.Add(message ?? throw new ArgumentNullException(nameof(message)));
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{_messages.Count} event(s)";                
    }
}
