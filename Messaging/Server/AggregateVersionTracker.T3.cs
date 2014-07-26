namespace System.ComponentModel.Messaging.Server
{
    internal sealed class AggregateVersionTracker<TKey, TVersion, TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>
        where TAggregate : class, IAggregate<TKey, TVersion>
    {
        private readonly TVersion _originalVersion;
        private readonly TAggregate _aggregate;

        public AggregateVersionTracker(TAggregate aggregate)
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
