namespace System.ComponentModel.Server.Domain
{
    internal sealed class AggregateRootVersionTracker<TAggregate, TKey, TVersion>
        where TAggregate : class, IVersionedObject<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {
        private readonly TVersion _originalVersion;
        private readonly TAggregate _aggregate;

        public AggregateRootVersionTracker(TAggregate aggregate)
        {
            _originalVersion = aggregate.Version;
            _aggregate = aggregate;
        }

        public TVersion OriginalVersion
        {
            get { return _originalVersion; }
        }

        public TAggregate Aggregate
        {
            get { return _aggregate; }
        }        
    }
}
