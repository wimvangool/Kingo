using System;
using System.Collections.Generic;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a simple, in-memory <see cref="IEventBus"/> implementation.
    /// </summary>
    public sealed class EventBus : IEventBus
    {
        private readonly List<object> _messages;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBus" /> class.
        /// </summary>
        public EventBus()
        {
            _messages = new List<object>();
        }

        /// <inheritdoc />
        public void Publish(object message) =>
            _messages.Add(message ?? throw new ArgumentNullException(nameof(message)));

        /// <inheritdoc />
        public MessageStream ToStream() =>
            MessageStream.CreateStream(_messages);

        /// <inheritdoc />
        public override string ToString() =>
            $"{_messages.Count} event(s)";
    }
}
