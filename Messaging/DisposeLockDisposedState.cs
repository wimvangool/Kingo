using System.ComponentModel.Resources;
using System.Threading;

namespace System.ComponentModel
{
    internal sealed class DisposeLockDisposedState : DisposeLockState
    {
        private readonly object _instance;        
        private readonly ThreadLocal<int> _enterDisposeCount;

        internal DisposeLockDisposedState(object instance, ThreadLocal<int> enterDisposeCount)
        {
            _instance = instance;            
            _enterDisposeCount = enterDisposeCount;
        }

        internal override bool IsDisposed
        {
            get { return true; }
        }

        protected override object Instance
        {
            get { return _instance; }
        }        

        internal override void EnterDispose(ref DisposeLockState currentState)
        {
            _enterDisposeCount.Value++;
        }

        internal override void ExitDispose(ref DisposeLockState currentState)
        {
            if (_enterDisposeCount.Value == 0)
            {
                throw NewEnterDisposeNotInvokedException();
            }
            _enterDisposeCount.Value--;
        }

        internal override void EnterMethod()
        {
            throw NewObjectDisposedException();
        }

        internal override void ExitMethod()
        {
            throw NewEnterMethodNotInvokedException();
        }

        private static Exception NewEnterMethodNotInvokedException()
        {
            return new SynchronizationLockException(ExceptionMessages.DisposeLock_EnterMethodNotInvoked);
        }
    }
}
