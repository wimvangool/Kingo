
namespace YellowFlare.MessageProcessing
{    
    internal sealed class UnitOfWorkWithAttributeOneSync : IUnitOfWork
    {
        private readonly IUnitOfWork _flushable;

        public UnitOfWorkWithAttributeOneSync(IUnitOfWork flushable)
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
