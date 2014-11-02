using System.ComponentModel.Resources;

namespace System.ComponentModel.Client.DataVirtualization
{
    /// <summary>
    /// Arguments of the <see cref="IVirtualCollectionPageLoader{T}.PageFailedToLoad" /> event.
    /// </summary>
    public class PageFailedToLoadEventArgs : EventArgs
    {
        /// <summary>
        /// The index of the page that failed to load.
        /// </summary>
        public readonly int PageIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageFailedToLoadEventArgs" /> class.
        /// </summary>
        /// <param name="pageIndex">The index of the item that failed to load.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="pageIndex"/> is negative.
        /// </exception>
        public PageFailedToLoadEventArgs(int pageIndex)
        {
            if (pageIndex < 0)
            {
                throw NewIndexOutOfRangeException(pageIndex);
            }
            PageIndex = pageIndex;
        }

        private static Exception NewIndexOutOfRangeException(int pageIndex)
        {
            var messageFormat = ExceptionMessages.Object_IndexOutOfRange;
            var message = string.Format(messageFormat, pageIndex);
            return new ArgumentOutOfRangeException("pageIndex", message);
        }
    }
}
