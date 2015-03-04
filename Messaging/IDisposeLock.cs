using System.Threading;

namespace System.ComponentModel
{
    /// <summary>
    /// When implemented by a class, represents a lock that ensures instance methods of
    /// <see cref="IDisposable" /> instances are safely entered and exitted in multi-threaded context.
    /// </summary>
    public interface IDisposeLock
    {
        /// <summary>
        /// Indicates whether or not the lock is in the disposed state.
        /// </summary>
        bool IsDisposed
        {
            get;
        }

        /// <summary>
        /// Signals to the lock that an instance method is entered on the managed instance.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The instance has already been disposed by another thread.
        /// </exception>
        void EnterMethod();

        /// <summary>
        /// Signals to the lock that an instance method is exitted on the managed instance.
        /// </summary>
        /// <exception cref="SynchronizationLockException">
        /// <see cref="EnterMethod()"/> has not been invoked first by the current thread.
        /// </exception>
        void ExitMethod();
    }
}
