namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as a base class for modules that are part of a <see cref="IQuery{TMessageIn, TMessageOut}" />-pipeline.
    /// </summary>
    public abstract class QueryModule : IQueryModule
    {
        #region [====== Dispose ======]

        /// <summary>
        /// Indicates whether not this instance has been disposed.
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
            if (IsDisposed)
            {
                return;
            }            
            IsDisposed = true;
        }

        /// <summary>
        /// Creates and returns a new <see cref="ObjectDisposedException" />.
        /// </summary>
        /// <returns>A new <see cref="ObjectDisposedException" />.</returns>
        protected ObjectDisposedException NewObjectDisposedException()
        {
            return new ObjectDisposedException(GetType().Name);
        }

        #endregion

        /// <inheritdoc />
        public TMessageOut Invoke<TMessageOut>(IQuery<TMessageOut> query) where TMessageOut : class, IMessage<TMessageOut>
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            return InvokeQuery(query);
        }

        /// <summary>
        /// Executes the specified <paramref name="query"/> while adding specific pipeline logic.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the result of <paramref name="query"/>.</typeparam>
        /// <param name="query">The handler to execute.</param>
        /// <returns>The result of the <paramref name="query"/>.</returns>
        protected abstract TMessageOut InvokeQuery<TMessageOut>(IQuery<TMessageOut> query) where TMessageOut : class, IMessage<TMessageOut>;
    }
}
