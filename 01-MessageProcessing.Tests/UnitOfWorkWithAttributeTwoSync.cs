
namespace YellowFlare.MessageProcessing
{
    [FlushHint(Group = "Two", ForceSynchronousFlush = true)]
    internal sealed class UnitOfWorkWithAttributeTwoSync : IUnitOfWork
    {
        private readonly IUnitOfWork _flushable;

        public UnitOfWorkWithAttributeTwoSync(IUnitOfWork flushable)
        {
            _flushable = flushable;
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
