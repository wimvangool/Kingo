
namespace YellowFlare.MessageProcessing.Server
{    
    internal sealed class UnitOfWorkOneSyncTest : IUnitOfWork
    {
        private readonly IUnitOfWork _flushable;

        public UnitOfWorkOneSyncTest(IUnitOfWork flushable)
        {
            _flushable = flushable;
        }

        public string FlushGroup
        {
            get { return "One"; }
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
