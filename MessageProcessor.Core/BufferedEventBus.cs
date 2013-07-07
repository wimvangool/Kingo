using System.Collections.Generic;

namespace YellowFlare.MessageProcessing
{
    public sealed class BufferedEventBus : IDomainEventBus, IUnitOfWork
    {        
        private readonly List<IBufferedEvent> _buffer;
        private readonly IDomainEventBus _domainEventBus;

        internal BufferedEventBus(IDomainEventBus domainEventBus)
        {                        
            _buffer = new List<IBufferedEvent>(4);
            _domainEventBus = domainEventBus;
        }

        string IUnitOfWork.FlushGroup
        {
            get { return typeof(BufferedEventBus).FullName; }
        }

        bool IUnitOfWork.CanBeFlushedAsynchronously
        {
            get { return false; }
        }

        void IDomainEventBus.Publish<TMessage>(TMessage message)
        {
            _buffer.Add(new BufferedEvent<TMessage>(message));
        }

        bool IUnitOfWork.RequiresFlush()
        {
            return _buffer.Count > 0;
        }

        void IUnitOfWork.Flush()
        {
            foreach (var bufferedEvent in _buffer)
            {
                bufferedEvent.Publish(_domainEventBus);
            }
            _buffer.Clear();
        }

        public static bool Publish<TMessage>(TMessage message) where TMessage : class
        {
            var context = UnitOfWorkContext.Current;
            if (context == null)
            {
                return false;
            }
            context.PeekBus().Publish(message);
            return true;
        }        
    }
}
