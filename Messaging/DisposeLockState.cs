using System.ComponentModel.Resources;
using System.Threading;

namespace System.ComponentModel
{
    internal abstract class DisposeLockState
    {
        protected abstract object Instance
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

        internal abstract void EnterDispose(ref DisposeLockState currentState);

        internal abstract void ExitDispose(ref DisposeLockState currentState);        

        internal abstract void EnterMethod();

        internal abstract void ExitMethod();

        protected bool TryMoveToState(ref DisposeLockState currentState, DisposeLockState newState)
        {
            return ReferenceEquals(Interlocked.CompareExchange(ref currentState, newState, this), this);
        }

        protected static SynchronizationLockException NewEnterDisposeNotInvokedException()
        {
            return new SynchronizationLockException(ExceptionMessages.DisposeLock_EnterDisposeNotInvoked);
        }
    }
}
