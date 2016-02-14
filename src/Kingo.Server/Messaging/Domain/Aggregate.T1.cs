using System;
using System.Threading.Tasks;

namespace Kingo.Messaging.Domain
{
    internal abstract class Aggregate<TKey, TVersion, TAggregate>        
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IHasKeyAndVersion<TKey, TVersion>
    {
        internal abstract TAggregate Value
        {
            get;
        }

        internal abstract bool Matches(TAggregate aggregate);

        internal abstract bool HasBeenUpdated();        

        internal abstract Task CommitAsync(IWritableEventStream<TKey, TVersion> domainEventStream);
    }
}
