using System;
using System.Threading.Tasks;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a <see cref="Repository{T, K, S}" /> that stores its aggregates as a stream of events.
    /// </summary>
    /// <typeparam name="TKey">Type of the key that identifies an aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of aggregates that are managed.</typeparam>
    public abstract class EventStreamRepository<TKey, TVersion, TAggregate> : Repository<TKey, TVersion, TAggregate>        
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IVersionedObject<TKey, TVersion>, IReadableEventStream<TKey, TVersion>
    {
        #region [====== Getting & Updating ======]

        internal override Task UpdateAsync(TAggregate aggregate, TVersion originalVersion, IWritableEventStream<TKey, TVersion> domainEventStream)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [====== Adding & Inserting ======]

        internal override Task InsertAsync(TAggregate aggregate, IWritableEventStream<TKey, TVersion> domainEventStream)
        {
            throw new NotImplementedException();
        }

        #endregion        
    }
}
