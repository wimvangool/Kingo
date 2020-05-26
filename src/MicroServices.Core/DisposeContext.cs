using System;

namespace Kingo
{
    /// <summary>
    /// Indicates in what context an object is disposed when it implements <see cref="IAsyncDisposable" />.
    /// </summary>
    public enum DisposeContext
    {
        /// <summary>
        /// Indicates the <see cref="Disposable.Dispose(bool)"/> method was called by the finalizer.
        /// </summary>
        Finalizer,

        /// <summary>
        /// Indicates the <see cref="Disposable.Dispose()"/> method was called on the object.
        /// </summary>
        Synchronous,

        /// <summary>
        /// Indicates the <see cref="AsyncDisposable.DisposeAsync()"/> method was called on the object.
        /// </summary>
        Asynchronous
    }
}
