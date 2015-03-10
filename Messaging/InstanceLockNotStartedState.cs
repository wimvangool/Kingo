using System.ComponentModel.Resources;
using System.Threading;

namespace System.ComponentModel
{
    internal sealed class InstanceLockNotStartedState : InstanceLockState
    {
        private readonly object _instance;
        private readonly ReaderWriterLockSlim _internalLock;

        internal InstanceLockNotStartedState(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            _instance = instance;
            _internalLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        protected override object Instance
        {
            get { return _instance; }
        }

        internal override bool IsStarted
        {
            get { return false; }
        }

        internal override bool IsDisposed
        {
            get { return false; }
        }

        internal override void Start(ref InstanceLockState currentState)
        {
            if (TryMoveToState(ref currentState, new InstanceLockStartedState(_instance, _internalLock)))
            {
                return;
            }
            throw NewInstanceAlreadyStartedException();
        }

        internal override void EnterDispose(ref InstanceLockState currentState)
        {
            // Only the first thread that calls EnterDispose ever gets a hold of the WriteLock.
            // This has the advantage that other threads won't be blocked when also calling
            // EnterDispose and that only one thread 'owns' the lock so that it can safely
            // be disposed.
            if (TryMoveToState(ref currentState, new InstanceLockDisposingState(_instance, _internalLock)))
            {
                _internalLock.EnterWriteLock();
            }
            else
            {
                // If the state has already been promoted another thread that came first, a race condition
                // occurred and we simply call EnterDispose on the new state.
                currentState.EnterDispose(ref currentState);
            }
        }

        internal override void ExitDispose(ref InstanceLockState currentState)
        {
            throw NewEnterDisposeNotInvokedException();
        }

        internal override void EnterMethod()
        {
            throw NewNotYetStartedException();
        }

        internal override void ExitMethod()
        {
            throw NewEnterMethodNotInvokedException();
        }

        private static Exception NewNotYetStartedException()
        {
            return new InvalidOperationException(ExceptionMessages.InstanceLock_NotYetStarted);
        }       
    }
}
