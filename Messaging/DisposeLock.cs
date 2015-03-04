using System.Threading;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a lock that can be used for mutli-threaded objects that implement <see cref="IDisposable" />
    /// to ensure <see cref="IDisposable.Dispose()" /> is implemented in a thread-safe manner.
    /// </summary>
    public sealed class DisposeLock : IDisposeLock, IDisposable
    {                
        private DisposeLockState _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposeLock" /> class.
        /// </summary>
        /// <param name="instance">The instance for which this lock is used.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <c>null</c>.
        /// </exception>
        public DisposeLock(object instance)
        {                       
            _state = new DisposeLockActiveState(instance);
        }

        /// <inheritdoc />
        public bool IsDisposed
        {
            get { return _state.IsDisposed; }
        }

        void IDisposable.Dispose()
        {
            EnterDispose();
            ExitDispose();
        }

        /// <summary>
        /// Marks the entry of the <see cref="IDisposable.Dispose()" /> method.
        /// </summary>        
        /// <exception cref="LockRecursionException">
        /// The current thread has already entered a method using <see cref="EnterMethod()" />.
        /// </exception>
        public void EnterDispose()
        {
            _state.EnterDispose(ref _state);
        }

        /// <summary>
        /// Marks the exit of the <see cref="IDisposable.Dispose()" /> method.
        /// </summary>
        /// <exception cref="SynchronizationLockException">
        /// <see cref="EnterDispose()"/> has not been called by the current thread, or
        /// the current thread has entered a method by calling <see cref="EnterMethod()" />
        /// but did not yet exit this method by invoking <see cref="ExitMethod()" />.
        /// </exception>
        public void ExitDispose()
        {
            _state.ExitDispose(ref _state);
        }

        /// <inheritdoc />
        public void EnterMethod()
        {
            _state.EnterMethod();
        }

        /// <inheritdoc />
        public void ExitMethod()
        {
            _state.ExitMethod();
        }
    }
}
