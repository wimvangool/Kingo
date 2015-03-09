using System.Threading;

namespace System.ComponentModel
{
    internal sealed class InstanceLockStartedState : InstanceLockState
    {
        private readonly object _instance;
        private readonly ReaderWriterLockSlim _internalLock;

        internal InstanceLockStartedState(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            _instance = instance;
            _internalLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        internal InstanceLockStartedState(object instance, ReaderWriterLockSlim internalLock)
        {            
            _instance = instance;
            _internalLock = internalLock;
        }

        internal override bool IsStarted
        {
            get { return true; }
        }

        internal override bool IsDisposed
        {
            get { return false; }
        }

        protected override object Instance
        {
            get { return _instance; }
        }

        internal override void Start(ref InstanceLockState currentState)
        {
            throw NewInstanceAlreadyStartedException();
        }

        internal override void EnterDispose(ref InstanceLockState currentState)
        {
            // Only the first thread that calls EnterDispose ever gets a hold of the WriteLock.
            // This has the advantage that other threads won't be blocked when also calling
            // EnterDispose and that only one thread 'owns' the lock so that it can safely
            // be disposed.
            if (TryMoveToState(ref currentState, new InstanceLockDisposingState(Instance, _internalLock)))
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
            _internalLock.EnterReadLock();            
        }

        internal override void ExitMethod()
        {
            _internalLock.ExitReadLock();            
        }
    }
}
