using System;
using System.Threading.Tasks;

namespace Kingo
{
    /// <summary>
    /// Serves as a base-class for object that want to implement the <see cref="IAsyncDisposable" /> interface.
    /// </summary>
    public abstract class AsyncDisposable : Disposable, IAsyncDisposable
    {
        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);

            await DisposeAsync(DisposeContext.Asynchronous);
        }

        /// <inheritdoc />
        protected sealed override async void Dispose(bool disposing)
        {
            try
            {
                await DisposeAsync(disposing ? DisposeContext.Synchronous : DisposeContext.Finalizer);
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Disposes this object asynchronously.
        /// </summary>
        /// <param name="context">The context in which this method is running.</param>
        /// <returns>A task representing the operation.</returns>
        protected virtual ValueTask DisposeAsync(DisposeContext context)
        {
            if (IsDisposed)
            {
                return default;
            }
            if (context == DisposeContext.Asynchronous)
            {
                base.Dispose(true);
            }
            return default;
        }
    }
}
