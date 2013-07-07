
namespace YellowFlare.MessageProcessing
{    
    internal sealed class UnitOfWorkWithAttributeTwoSync : IUnitOfWork
    {
        private readonly IUnitOfWork _flushable;

        public UnitOfWorkWithAttributeTwoSync(IUnitOfWork flushable)
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
