﻿namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents a buffered stream of events that can be flushed to another stream.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate's key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate's version.</typeparam>
    public interface IBufferedEventStream<out TKey, out TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>
    {
        /// <summary>
        /// Flushed this stream to the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to flush to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.
        /// </exception>
        void FlushTo(IWritableEventStream<TKey, TVersion> stream);
    }
}
