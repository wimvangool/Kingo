namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// Represents a cache-entry that can be invalidated.
    /// </summary>
    public abstract class CacheEntry
    {
        private readonly object _messageIn;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheEntry" /> class.
        /// </summary>
        /// <param name="messageIn">The key of the stored cache-entry.</param>
        protected CacheEntry(object messageIn)
        {
            if (messageIn == null)
            {
                throw new ArgumentNullException("messageIn");
            }
            _messageIn = messageIn;
        }

        /// <summary>
        /// The key of the cache-entry.
        /// </summary>
        public object MessageIn
        {
            get { return _messageIn; }
        }
        
        internal void InvalidatePostCommit()
        {
            MessageProcessor.InvokePostCommit(Invalidate);
        }

        /// <summary>
        /// Invalidates the cache-entry, which will eventually result in eviction of the entry from the cache.
        /// </summary>
        protected abstract void Invalidate(bool isPostCommit);
    }
}
