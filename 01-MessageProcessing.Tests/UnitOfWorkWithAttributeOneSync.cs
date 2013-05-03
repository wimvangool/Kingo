﻿
namespace YellowFlare.MessageProcessing
{
    [FlushHint(Group = "One", ForceSynchronousFlush = true)]
    internal sealed class UnitOfWorkWithAttributeOneSync : IUnitOfWork
    {
        private readonly IUnitOfWork _flushable;

        public UnitOfWorkWithAttributeOneSync(IUnitOfWork flushable)
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
