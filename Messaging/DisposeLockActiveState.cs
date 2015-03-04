using System.Threading;

namespace System.ComponentModel
{
    internal sealed class DisposeLockActiveState : DisposeLockState
    {
        private readonly object _instance;
        private readonly ReaderWriterLockSlim _internalLock;

        internal DisposeLockActiveState(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            _instance = instance;
            _internalLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        internal override bool IsDisposed
        {
            get { return false; }
        }

        protected override object Instance
        {
            get { return _instance; }
        }        

        internal override void EnterDispose(ref DisposeLockState currentState)
        {
            // Only the first thread that calls EnterDispose ever gets a hold of the WriteLock.
            // This has the advantage that other threads won't be blocked when also calling
            // EnterDispose and that only one thread 'owns' the lock so that it can safely
            // be disposed.
            if (TryMoveToState(ref currentState, new DisposeLockDisposingState(Instance, _internalLock)))
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

        internal override void ExitDispose(ref DisposeLockState currentState)
        {
            throw NewEnterDisposeNotInvokedException();
        }

        internal override void EnterMethod()
        {
            _internalLock.EnterReadLock();            
        }

        internal override void ExitMethod()
        {
            _internalLock.ExitReadLock();            
        }
    }
}
