using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a stream of <see cref="IVersionedObject{T, S}" /> that can be written to.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate's key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate's version.</typeparam>
    public interface IWritableEventStream<in TKey, in TVersion>        
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// Writes the specified <paramref name="event"/> to the current stream.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="event">The event to write to this stream.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="event"/> is <c>null</c>.
        /// </exception>
        void Write<TEvent>(TEvent @event) where TEvent : class, IVersionedObject<TKey, TVersion>, IMessage;
    }
}
