namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
    /// <summary>
    /// Arguments of the <see cref="IVirtualCollectionPageLoader{T}.PageLoaded" /> event.
    /// </summary>
    /// <typeparam name="T">Type of the item that was loaded.</typeparam>
    public class PageLoadedEventArgs<T> : EventArgs
    {        
        /// <summary>
        /// The page that was loaded.
        /// </summary>
        public readonly VirtualCollectionPage<T> Page;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageLoadedEventArgs{T}" /> class.
        /// </summary>        
        /// <param name="page">The page that was loaded.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="page"/> is <c>null</c>.
        /// </exception>
        public PageLoadedEventArgs(VirtualCollectionPage<T> page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }            
            Page = page;
        }        
    }
}
