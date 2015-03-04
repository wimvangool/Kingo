namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as a base class for modules that are part of a <see cref="IQuery{TMessageIn, TMessageOut}" />-pipeline.
    /// </summary>
    public abstract class QueryModule : IQueryModule
    {
        private readonly DisposeLock _disposeLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryModule" /> class.
        /// </summary>
        protected QueryModule()
        {
            _disposeLock = new DisposeLock(this);
        }

        #region [====== Dispose ======]

        /// <summary>
        /// Returns the lock that is used to manage safe disposal of this instance.
        /// </summary>
        protected IDisposeLock DisposeLock
        {
            get { return _disposeLock; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _disposeLock.EnterDispose();

            try
            {
                Dispose(true);
            }
            finally
            {
                _disposeLock.ExitDispose();
            }            
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
        protected virtual void Dispose(bool disposing) { }       

        #endregion

        /// <inheritdoc />
        public TMessageOut Invoke<TMessageOut>(IQuery<TMessageOut> query) where TMessageOut : class, IMessage<TMessageOut>
        {            
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            DisposeLock.EnterMethod();

            try
            {
                return InvokeQuery(query);
            }
            finally
            {
                DisposeLock.ExitMethod();
            }            
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
