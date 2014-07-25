
namespace System.ComponentModel.Messaging.Server
{    
    internal sealed class UnitOfWorkOneAsyncTest : IUnitOfWork
    {
        private readonly IUnitOfWork _flushable;

        public UnitOfWorkOneAsyncTest(IUnitOfWork flushable)
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
