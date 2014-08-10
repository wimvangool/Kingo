using System.ComponentModel.Messaging.Resources;

namespace System.ComponentModel.Messaging
{
    /// <summary>
    /// Provides a basic implementation of the <see cref="IConnection" /> interface and can be used
    /// as a base-class for concrete implementations.
    /// </summary>
    public abstract class Connection : IConnection
    {
        #region [====== Dispose ======]

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
            if (IsDisposed)
            {
                return;
            }
            if (disposing && IsOpen)
            {
                Close();
            }
            IsDisposed = true;
        }

        #endregion

        #region [====== Opening and Closing ======]

        /// <summary>
        /// Indicates whether or not this connected has been opened.
        /// </summary>
        protected abstract bool IsOpen
        {
            get;
        }

        /// <inheritdoc />
        public void Open()
        {
            if (IsDisposed)
            {
                throw NewConnectionAlreadyDisposedException(this);
            }
            if (IsOpen)
            {
                throw NewConnectionAlreadyOpenException();
            }
            OpenConnection();
        }

        /// <summary>
        /// Opens this connections.
        /// </summary>
        protected abstract void OpenConnection();

        /// <inheritdoc />
        public void Close()
        {
            if (IsDisposed)
            {
                throw NewConnectionAlreadyDisposedException(this);
            }
            if (IsOpen)
            {
                CloseConnection();
                return;
            }
            throw NewConnectionAlreadyClosedException();
        }        

        /// <summary>
        /// Closes this connection.
        /// </summary>
        protected abstract void CloseConnection();

        #endregion

        #region [====== Exception Factory Methods ======]

        private static Exception NewConnectionAlreadyDisposedException(object connection)
        {
            return new ObjectDisposedException(connection.GetType().Name);
        }

        private static Exception NewConnectionAlreadyOpenException()
        {
            return new InvalidOperationException(ExceptionMessages.Connection_AlreadyOpen);
        }

        private static Exception NewConnectionAlreadyClosedException()
        {
            return new InvalidOperationException(ExceptionMessages.Connection_AlreadyClosed);
        }

        #endregion
    }
}
