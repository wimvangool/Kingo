using System;

namespace Kingo
{
    /// <summary>
    /// Serves as a base-class for disposable objects.
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        /// <summary>
        /// Indicates whether or not an object has been disposed.
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
        protected void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                DisposeManagedResources();
            }
            DisposeUnmanagedResources();
            IsDisposed = true;
        }

        /// <summary>
        /// Disposes all managed resources owned by this instance.
        /// </summary>
        protected virtual void DisposeManagedResources() { }

        /// <summary>
        /// Disposed all unmanaged resources own by this instance.
        /// </summary>
        protected virtual void DisposeUnmanagedResources() { }

        /// <summary>
        /// Creates and returns a new <see cref="ObjectDisposedException" /> indicating this instance has been disposed.
        /// </summary>
        /// <returns>A new <see cref="ObjectDisposedException" />.</returns>
        protected virtual ObjectDisposedException NewObjectDisposedException()
        {
            return new ObjectDisposedException(GetType().Name);
        }
    }
}
