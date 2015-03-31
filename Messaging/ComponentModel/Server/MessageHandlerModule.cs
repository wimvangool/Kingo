using System.Threading;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as the base-class for all modules in a <see cref="IMessageHandler{TMessage}" /> pipeline.
    /// </summary>
    public abstract class MessageHandlerModule : IMessageProcessorPipeline
    {
        private readonly InstanceLock _disposeLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerModule" /> class.
        /// </summary>
        protected MessageHandlerModule()
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
        /// Invokes the specified <paramref name="handler"/> while adding specific pipeline logic.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        /// <exception cref="InvalidOperationException">
        /// The module has not yet been started.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// This instance has been disposed.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public void Invoke(IMessageHandler handler)
        {            
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            InstanceLock.EnterMethod();

            try
            {
                InvokeHandler(handler);
            }
            finally
            {
                InstanceLock.ExitMethod();
            }            
        }

        /// <summary>
        /// Invokes the specified <paramref name="handler"/> while adding specific pipeline logic.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        protected abstract void InvokeHandler(IMessageHandler handler);
    }
}
