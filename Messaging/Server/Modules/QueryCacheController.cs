namespace System.ComponentModel.Server.Modules
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IQueryCacheController" /> interface.
    /// </summary>
    public abstract class QueryCacheController : IQueryCacheController
    {
        #region [====== Dispose ======]

        /// <summary>
        /// Indicates whether or not the current instance has been disposed.
        /// </summary>
        protected bool IsDisposed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            IsDisposed = true;
        }

        protected ObjectDisposedException NewObjectDisposedException()
        {
            return new ObjectDisposedException(GetType().Name);
        }

        #endregion

        #region [====== Caching ======]                        

        /// <inheritdoc /> 
        public abstract TMessageOut GetOrAddToApplicationCache<TMessageIn, TMessageOut>(QueryRequestMessage<TMessageIn> message, IQuery<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>;

        /// <inheritdoc />       
        public abstract TMessageOut GetOrAddToSessionCache<TMessageIn, TMessageOut>(QueryRequestMessage<TMessageIn> message, IQuery<TMessageIn, TMessageOut> query)
            where TMessageIn : class, IMessage<TMessageIn>;

        /// <inheritdoc />      
        public abstract void InvalidateIfRequired<TMessageIn>(Func<TMessageIn, bool> mustInvalidate) where TMessageIn : class;

        #endregion
    }
}
