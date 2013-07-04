using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing.Aggregates
{
    public sealed class DomainEventBusAdapter<TKey> : IWritableEventStream<TKey> where TKey : struct, IEquatable<TKey>
    {
        private readonly IDomainEventBus _domainEventBus;

        public DomainEventBusAdapter(IDomainEventBus domainEventBus)
        {
            if (domainEventBus == null)
            {
                throw new ArgumentNullException("domainEventBus");
            }
            _domainEventBus = domainEventBus;
        }

        public void PublishEvents(IEnumerable<IBufferedEventStream<TKey>> aggregates)
        {
            if (aggregates == null)
            {
                throw new ArgumentNullException("aggregates");
            }
            foreach (var aggregate in aggregates)
            {
                aggregate.FlushTo(this);
            }
        }

        void IWritableEventStream<TKey>.Write<TDomainEvent>(TDomainEvent domainEvent)
        {
            _domainEventBus.Publish(domainEvent);
        }
    }
}
