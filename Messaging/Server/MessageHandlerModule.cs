namespace System.ComponentModel.Server
{
    /// <summary>
    /// Provides a base-implementation of the <see cref="IMessageHandlerModule" /> interface.
    /// </summary>
    public abstract class MessageHandlerModule : IMessageHandlerModule
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
            if (disposing)
            {
                // Dispose managed resources here...
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
        public void Invoke(IMessageHandler handler)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            InvokeHandler(handler);
        }

        /// <summary>
        /// Invokes the specified <paramref name="handler"/> while adding specific pipeline logic.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        protected abstract void InvokeHandler(IMessageHandler handler);
    }
}
