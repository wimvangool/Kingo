using System.Collections.Generic;

namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// Represents an in-memory stream of <see cref="IAggregateEvent{T, S}">aggregate events</see>.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate's key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate's version.</typeparam>
    public sealed class MemoryEventStream<TKey, TVersion> : IEventStream<TKey, TVersion>, IWritableEventStream<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        private readonly List<IEventBuffer<TKey, TVersion>> _buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryEventStream{T, S}" /> class.
        /// </summary>
        public MemoryEventStream()
        {
            _buffer = new List<IEventBuffer<TKey, TVersion>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryEventStream{T, S}" /> class.
        /// </summary>
        /// <param name="capacity">The initial capacity of this buffer.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is negative.
        /// </exception>
        public MemoryEventStream(int capacity)
        {
            _buffer = new List<IEventBuffer<TKey, TVersion>>(capacity);
        }

        /// <summary>
        /// Appends the specified event to this stream.
        /// </summary>
        /// <typeparam name="TEvent">Type of event to append.</typeparam>
        /// <param name="event">Event to append.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        public void Write<TEvent>(TEvent @event) where TEvent : class, IAggregateEvent<TKey, TVersion>
        {
            if (@event == null)
            {
                throw new ArgumentNullException("event");
            }
            _buffer.Add(new EventBuffer<TKey, TVersion, TEvent>(@event));
        }

        /// <summary>
        /// Flushes all events of this stream to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to flush to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.
        /// </exception>
        public void FlushTo(IWritableEventStream<TKey, TVersion> stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            foreach (var bufferedEvent in _buffer)
            {
                bufferedEvent.WriteTo(stream);
            }
            _buffer.Clear();
        }
    }
}
