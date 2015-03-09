using System.ComponentModel.Resources;
using System.Threading;

namespace System.ComponentModel
{
    internal sealed class InstanceLockDisposedState : InstanceLockState
    {
        private readonly object _instance;        
        private readonly ThreadLocal<int> _enterDisposeCount;

        internal InstanceLockDisposedState(object instance, ThreadLocal<int> enterDisposeCount)
        {
            _instance = instance;            
            _enterDisposeCount = enterDisposeCount;
        }

        internal override bool IsStarted
        {
            get { return false; }
        }

        internal override bool IsDisposed
        {
            get { return true; }
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
            _enterDisposeCount.Value++;
        }

        internal override void ExitDispose(ref InstanceLockState currentState)
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
    }
}
