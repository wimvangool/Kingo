using System;

namespace Kingo.Messaging.Domain
{
    internal sealed class AggregateRootVersionTracker<TAggregate, TKey, TVersion>
        where TAggregate : class, IVersionedObject<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {
        private readonly TVersion _originalVersion;
        private readonly TAggregate _aggregate;

        internal AggregateRootVersionTracker(TAggregate aggregate)
        {
            _originalVersion = aggregate.Version;
            _aggregate = aggregate;
        }

        internal TVersion OriginalVersion
        {
            get { return _originalVersion; }
        }

        internal TAggregate Aggregate
        {
            get { return _aggregate; }
        }
        
        internal AggregateRootVersionTracker<TAggregate, TKey, TVersion> CommitUpdates()
        {
            return new AggregateRootVersionTracker<TAggregate, TKey, TVersion>(_aggregate);
        }
    }
}
