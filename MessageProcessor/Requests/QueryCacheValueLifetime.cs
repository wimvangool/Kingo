using System;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Provides a base-implementation of the <see cref="IQueryCacheValueLifetime" /> interface.
    /// </summary>
    public abstract class QueryCacheValueLifetime : IQueryCacheValueLifetime
    {        
        #region [====== Disposing ======]

        /// <summary>
        /// Indicates whether or not this instance has been disposed.
        /// </summary>
        protected bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// Indicates if the method was called by the application explicitly (<c>true</c>), or by the finalizer
        /// (<c>false</c>).
        /// </param>
        /// <remarks>
        /// If <paramref name="disposing"/> is <c>true</c>, this method will dispose any managed resources immediately.
        /// Otherwise, only unmanaged resources will be released.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            IsDisposed = true;
        }

        #endregion

        /// <inheritdoc />
        public event EventHandler Expired;

        /// <summary>
        /// Raises the <see cref="Expired" /> event.
        /// </summary>
        protected virtual void OnIsExpired()
        {
            IsExpired = true;
            Expired.Raise(this);
        }

        /// <inheritdoc />
        public bool IsExpired
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public abstract void NotifyValueAccessed();        
    }
}
