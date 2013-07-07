
namespace YellowFlare.MessageProcessing
{    
    internal sealed class UnitOfWorkWithAttributeOneAsync : IUnitOfWork
    {
        private readonly IUnitOfWork _flushable;

        public UnitOfWorkWithAttributeOneAsync(IUnitOfWork flushable)
        {
            _flushable = flushable;
        }

        public string FlushGroup
        {
            get { return "One"; }
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
