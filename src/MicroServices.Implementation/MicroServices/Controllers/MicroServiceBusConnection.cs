using System.Threading;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IMicroServiceBusConnection"/> interface.
    /// </summary>
    public abstract class MicroServiceBusConnection : Disposable, IMicroServiceBusConnection
    {
        private readonly CancellationTokenSource _tokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusConnection" /> class.
        /// </summary>
        protected MicroServiceBusConnection()
        {
            _tokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Returns a token that is signaled when a request has been made to close this connection.
        /// </summary>
        protected CancellationToken CloseToken =>
            _tokenSource.Token;

        /// <summary>
        /// Indicates whether or not the connection has been closed.
        /// </summary>
        protected bool IsClosed
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public virtual void Close()
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (IsClosed)
            {
                return;
            }
            _tokenSource.Cancel();

            IsClosed = true;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                try
                {
                    Close();
                }
                finally
                {
                    _tokenSource.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
