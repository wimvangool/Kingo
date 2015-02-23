namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// Represents the arguments of the <see cref="QueryCacheManager.CacheEntryInserted" />
    /// and <see cref="QueryCacheManager.CacheEntryUpdated" /> events.
    /// </summary>
    public class CacheEntryInsertedOrUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// The key of the stored cache-entry.
        /// </summary>
        public readonly object MessageIn;

        /// <summary>
        /// The value of the stored cache-entry.
        /// </summary>
        public readonly object MessageOut;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheEntryInsertedOrUpdatedEventArgs" /> class.
        /// </summary>
        /// <param name="messageIn">The key of the stored cache-entry.</param>
        /// <param name="messageOut">The value of the stored cache-entry.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageIn"/> or <paramref name="messageOut"/> is <c>null</c>.
        /// </exception>
        public CacheEntryInsertedOrUpdatedEventArgs(object messageIn, object messageOut)
        {
            if (messageIn == null)
            {
                throw new ArgumentNullException("messageIn");
            }
            if (messageOut == null)
            {
                throw new ArgumentNullException("messageOut");
            }
            MessageIn = messageIn;
            MessageOut = messageOut;
        }
    }
}
