namespace System.ComponentModel.Client.DataVirtualization
{
    /// <summary>
    /// Arguments of the <see cref="VirtualCollection{T}.PageFailedToLoad" /> event.
    /// </summary>
    public class PageFailedToLoadEventArgs : EventArgs
    {
        /// <summary>
        /// The index of the page that failed to load.
        /// </summary>
        public readonly int PageIndex;

        /// <summary>
        /// The <see cref="AggregateException">Exception(s)</see> that was/were thrown
        /// while attempting to load the page.
        /// </summary>
        public readonly AggregateException Exception;
        
        internal PageFailedToLoadEventArgs(int pageIndex, AggregateException exception)
        {            
            PageIndex = pageIndex;
            Exception = exception;
        }        
    }
}
