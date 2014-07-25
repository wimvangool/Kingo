
namespace System.ComponentModel.Messaging.Server
{    
    internal sealed class UnitOfWorkTwoSyncTest : IUnitOfWork
    {
        private readonly IUnitOfWork _flushable;

        public UnitOfWorkTwoSyncTest(IUnitOfWork flushable)
        {
            _flushable = flushable;
        }

        public string FlushGroup
        {
            get { return "Two"; }
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
