namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as a base class for modules that are part of a <see cref="IQuery{TMessageIn, TMessageOut}" />-pipeline.
    /// </summary>
    public abstract class QueryModule : IMessageProcessorPipeline
    {
        private readonly InstanceLock _disposeLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryModule" /> class.
        /// </summary>
        protected QueryModule()
        {
            _disposeLock = new InstanceLock(this);
        }

        /// <summary>
        /// Starts the module.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The module has already been started.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The module has already been disposed.
        /// </exception>
        public virtual void Start()
        {
            _disposeLock.Start();
        }

        #region [====== Dispose ======]

        /// <summary>
        /// Returns the lock that is used to manage safe disposal of this instance.
        /// </summary>
        protected IInstanceLock InstanceLock
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

        /// <summary>
        /// Executes the specified <paramref name="query"/> while adding specific pipeline logic.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the result of <paramref name="query"/>.</typeparam>
        /// <param name="query">The handler to execute.</param>        
        /// <returns>The result of the <paramref name="query"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// The module has not yet been started.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// This instance has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public TMessageOut Invoke<TMessageOut>(IQuery<TMessageOut> query) where TMessageOut : class, IMessage<TMessageOut>
        {            
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            InstanceLock.EnterMethod();

            try
            {
                return InvokeQuery(query);
            }
            finally
            {
                InstanceLock.ExitMethod();
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
