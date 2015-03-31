using System.ComponentModel.Resources;

namespace System.Threading
{
    internal sealed class InstanceLockDisposingState : InstanceLockState
    {
        private readonly object _instance;
        private readonly ReaderWriterLockSlim _internalLock;
        private readonly ThreadLocal<bool> _isDisposing;
        private readonly ThreadLocal<int> _enterDisposeCount;

        internal InstanceLockDisposingState(object instance, ReaderWriterLockSlim internalLock)
        {
            _instance = instance;
            _internalLock = internalLock;
            _isDisposing = new ThreadLocal<bool>() { Value = true };
            _enterDisposeCount = new ThreadLocal<int>() { Value = 1 };
        }

        internal override bool IsStarted
        {
            get { return false; }
        }

        internal override bool IsDisposed
        {
            get { return !_internalLock.IsReadLockHeld && !_internalLock.IsWriteLockHeld; }
        }

        protected override object Instance
        {
            get { return _instance; }
        }

        internal override void Start(ref InstanceLockState currentState)
        {
            throw NewObjectDisposedException();
        }

        internal override void EnterDispose(ref InstanceLockState currentState)
        {
            // Only the thread that has already entered the write-lock is allowed
            // to enter it again (recursively).
            if (_isDisposing.Value)
            {
                _internalLock.EnterWriteLock();
            }
            _enterDisposeCount.Value++;
        }

        internal override void ExitDispose(ref InstanceLockState currentState)
        {
            // Along the same lines, only the thread that already entered the write-lock
            // (possibly recursively) is allowed to exit the write-lock and move the lock
            // into the disposed state.
            if (_internalLock.IsReadLockHeld)
            {
                throw NewExitMethodNotInvokedException();
            }
            if (_isDisposing.Value)
            {
                _internalLock.ExitWriteLock();

                if (_internalLock.IsWriteLockHeld)
                {
                    return;
                }
                TryMoveToState(ref currentState, new InstanceLockDisposedState(Instance, _enterDisposeCount));

                _internalLock.Dispose();
            }
            else if (_enterDisposeCount.Value == 0)
            {
                throw NewEnterDisposeNotInvokedException();
            }
            _enterDisposeCount.Value--;
        }

        internal override void EnterMethod()
        {
            if (_internalLock.IsReadLockHeld || _internalLock.IsWriteLockHeld)
            {
                _internalLock.EnterReadLock();
                return;
            }
            throw NewObjectDisposedException();
        }

        internal override void ExitMethod()
        {
            _internalLock.ExitReadLock();
        }

        private static Exception NewExitMethodNotInvokedException()
        {
            return new SynchronizationLockException(ExceptionMessages.InstanceLock_ExitMethodNotInvoked);
        }
    }
}
