using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// Represents a basic bus that stores all the published events in memory.
    /// </summary>
    public class MemoryServiceBus : MicroServiceBus, IReadOnlyList<object>
    {
        private readonly List<object> _events;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryServiceBus" /> class.
        /// </summary>
        public MemoryServiceBus()
        {
            _events = new List<object>();
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{_events.Count} event(s)";

        #region [====== IReadOnlyList<object> ======]

        /// <inheritdoc />
        public int Count =>
            _events.Count;

        /// <inheritdoc />
        public object this[int index] =>
            _events[index];

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<object> GetEnumerator() =>
            _events.GetEnumerator();

        #endregion

        #region [====== MicroServiceBus ======]

        /// <inheritdoc />
        public override Task PublishAsync(object message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            _events.Add(message);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Purges all events from memory.
        /// </summary>
        public void Purge() =>
            _events.Clear();

        #endregion
    }
}
