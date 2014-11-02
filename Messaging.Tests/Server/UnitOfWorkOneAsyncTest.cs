
namespace System.ComponentModel.Server
{    
    internal sealed class UnitOfWorkOneAsyncTest : IUnitOfWork
    {
        private readonly IUnitOfWork _flushable;
        private readonly Guid _flushGroupId;

        public UnitOfWorkOneAsyncTest(IUnitOfWork flushable, Guid flushGroupId)
        {
            _flushable = flushable;
            _flushGroupId = flushGroupId;
        }

        public Guid FlushGroupId
        {
            get { return _flushGroupId; }
        }

        public bool CanBeFlushedAsynchronously
        {
            get { return true; }
        }

        public bool RequiresFlush()
        {
            return _flushable.RequiresFlush();
        }

        public void Flush()
        {
            _flushable.Flush();
        }        
    }
}
