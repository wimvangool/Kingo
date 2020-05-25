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
        public virtual ValueTask DisposeAsync()
        {
            try
            {
                Dispose();
                return default;
            }
            catch (Exception exception)
            {
                return new ValueTask(Task.FromException(exception));
            }
        }
    }
}
