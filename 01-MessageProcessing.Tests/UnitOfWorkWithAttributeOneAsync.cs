
namespace YellowFlare.MessageProcessing
{
    [FlushHint(Group = "One")]
    internal sealed class UnitOfWorkWithAttributeOneAsync : IUnitOfWork
    {
        private readonly IUnitOfWork _flushable;

        public UnitOfWorkWithAttributeOneAsync(IUnitOfWork flushable)
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
