using System;
using System.Collections.Generic;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents an in-memory stream of <see cref="IVersionedObject{T, S}">aggregate events</see>.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate's key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate's version.</typeparam>
    [Serializable]
    public sealed class MemoryEventStream<TKey, TVersion> : IReadableEventStream<TKey, TVersion>, IWritableEventStream<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        private readonly Queue<IEventBuffer<TKey, TVersion>> _buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryEventStream{T, S}" /> class.
        /// </summary>
        public MemoryEventStream()
        {
            _buffer = new Queue<IEventBuffer<TKey, TVersion>>();
        }        

        /// <summary>
        /// Returns the number of events stored in this stream.
        /// </summary>
        public int Count
        {
            get { return _buffer.Count; }
        }

        /// <summary>
        /// Appends the specified event to this stream.
        /// </summary>
        /// <typeparam name="TEvent">Type of event to append.</typeparam>
        /// <param name="event">Event to append.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        public void Write<TEvent>(TEvent @event) where TEvent : class, IVersionedObject<TKey, TVersion>, IMessage<TEvent>
        {            
            _buffer.Enqueue(new EventBuffer<TKey, TVersion, TEvent>(@event));
        }

        /// <summary>
        /// Flushes all events of this stream to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to flush to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.
        /// </exception>
        public void WriteTo(IWritableEventStream<TKey, TVersion> stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            while (_buffer.Count > 0)
            {
                _buffer.Dequeue().WriteTo(stream);
            }          
        }
    }
}
