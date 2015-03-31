namespace System.ComponentModel.Server.Caching
{
    /// <summary>
    /// Represents the arguments of the <see cref="QueryCacheManager.CacheEntryDeleted" /> event.    
    /// </summary>
    public class CacheEntryDeletedEventArgs : EventArgs
    {
        /// <summary>
        /// The key of the deleted cache-entry.
        /// </summary>
        public readonly object MessageIn;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheEntryInsertedOrUpdatedEventArgs" /> class.
        /// </summary>
        /// <param name="messageIn">The key of the stored cache-entry.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageIn"/> is <c>null</c>.
        /// </exception>
        public CacheEntryDeletedEventArgs(object messageIn)
        {
            if (messageIn == null)
            {
                throw new ArgumentNullException("messageIn");
            }            
            MessageIn = messageIn;            
        }
    }
}
