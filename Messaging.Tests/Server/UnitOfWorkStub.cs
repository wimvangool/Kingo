using System.Threading;

namespace System.ComponentModel.Server
{    
    internal sealed class UnitOfWorkStub : IUnitOfWork, IDisposable
    {
        private bool _requiresFlush;

        public UnitOfWorkStub()
        {           
            Current = this;
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public bool IsFlushed
        {
            get;
            private set;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public void SimulateChange()
        {
            _requiresFlush = true;
        }

        public Guid FlushGroupId
        {
            get { return Guid.Empty; }
        }

        public bool CanBeFlushedAsynchronously
        {
            get { return false; }
        }

        public bool RequiresFlush()
        {
            return _requiresFlush;
        }

        public void Flush()
        {
            IsFlushed = true;
        }

        private static readonly ThreadLocal<UnitOfWorkStub> _Current = new ThreadLocal<UnitOfWorkStub>();

        public static UnitOfWorkStub Current
        {
            get { return _Current.Value; }
            set { _Current.Value = value; }
        }        
    }
}
