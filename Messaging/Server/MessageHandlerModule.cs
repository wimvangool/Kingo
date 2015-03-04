namespace System.ComponentModel.Server
{
    /// <summary>
    /// Provides a base-implementation of the <see cref="IMessageHandlerModule" /> interface.
    /// </summary>
    public abstract class MessageHandlerModule : IMessageHandlerModule
    {
        private readonly DisposeLock _disposeLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerModule" /> class.
        /// </summary>
        protected MessageHandlerModule()
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
        public void Invoke(IMessageHandler handler)
        {            
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            DisposeLock.EnterMethod();

            try
            {
                InvokeHandler(handler);
            }
            finally
            {
                DisposeLock.ExitMethod();
            }            
        }

        /// <summary>
        /// Invokes the specified <paramref name="handler"/> while adding specific pipeline logic.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        protected abstract void InvokeHandler(IMessageHandler handler);
    }
}
