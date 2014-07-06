using System;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents the lifetime of a value that is cached in a <see cref="IQueryCache" />.
    /// </summary>
    public interface IQueryCacheValueLifetime : IDisposable
    {
        /// <summary>
        /// Occurs when the lifetime has expired and the value can be removed from cache.
        /// </summary>
        /// <remarks>
        /// Note that this event may be raised on a different thread than this lifetime was created on.
        /// </remarks>
        event EventHandler Expired;

        /// <summary>
        /// Indicates whether or not this lifetime has expired.
        /// </summary>
        bool IsExpired
        {
            get;
        }

        /// <summary>
        /// Notifies the lifetime that the associated value has been accessed.
        /// </summary>
        void NotifyValueAccessed();        
    }
}
