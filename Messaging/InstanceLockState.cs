using System.ComponentModel.Resources;
using System.Threading;

namespace System.ComponentModel
{
    internal abstract class InstanceLockState
    {
        protected abstract object Instance
        {
            get;
        }

        internal abstract bool IsStarted
        {
            get;
        }

        internal abstract bool IsDisposed
        {
            get;
        }
               
        protected ObjectDisposedException NewObjectDisposedException()
        {
            return new ObjectDisposedException(Instance.GetType().Name);
        }

        internal abstract void Start(ref InstanceLockState currentState);

        internal abstract void EnterDispose(ref InstanceLockState currentState);

        internal abstract void ExitDispose(ref InstanceLockState currentState);        

        internal abstract void EnterMethod();

        internal abstract void ExitMethod();

        protected bool TryMoveToState(ref InstanceLockState currentState, InstanceLockState newState)
        {
            return ReferenceEquals(Interlocked.CompareExchange(ref currentState, newState, this), this);
        }

        protected static Exception NewInstanceAlreadyStartedException()
        {
            return new InvalidOperationException(ExceptionMessages.InstanceLock_AlreadyStarted);
        }

        protected static SynchronizationLockException NewEnterDisposeNotInvokedException()
        {
            return new SynchronizationLockException(ExceptionMessages.InstanceLock_EnterDisposeNotInvoked);
        }

        protected static Exception NewEnterMethodNotInvokedException()
        {
            return new SynchronizationLockException(ExceptionMessages.InstanceLock_EnterMethodNotInvoked);
        }
    }
}
