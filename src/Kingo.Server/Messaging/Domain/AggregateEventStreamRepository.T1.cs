using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a repository that stores its aggregates as a stream of events.
    /// </summary>    
    /// <typeparam name="TAggregate">Type of aggregates that are managed.</typeparam>
    public abstract class AggregateEventStreamRepository<TAggregate> : AggregateEventStreamRepository<Guid, int, TAggregate>
        where TAggregate : class, IAggregateRoot<Guid, int>, IWritableEventStream<Guid, int> { }
}
