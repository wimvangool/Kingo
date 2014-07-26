
namespace System.ComponentModel.Messaging.Server
{    
    internal sealed class UnitOfWorkOneSyncTest : IUnitOfWork
    {
        private readonly IUnitOfWork _flushable;
        private readonly Guid _flushGroupId;

        public UnitOfWorkOneSyncTest(IUnitOfWork flushable, Guid flushGroupId)
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
            get { return false; }
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
