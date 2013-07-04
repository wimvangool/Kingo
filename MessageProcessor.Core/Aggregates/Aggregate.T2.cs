using System;

namespace YellowFlare.MessageProcessing.Aggregates
{
    internal sealed class Aggregate<TKey, TValue>
        where TKey : struct, IEquatable<TKey>
        where TValue : class, IAggregate<TKey>
    {
        private readonly AggregateVersion _originalVersion;
        private readonly TValue _value;

        public Aggregate(TValue value)
        {
            _originalVersion = value.Version;
            _value = value;
        }

        public AggregateVersion OriginalVersion
        {
            get { return _originalVersion; }
        }

        public TValue Value
        {
            get { return _value; }
        }

        public bool HasBeenUpdated
        {
            get { return _value.Version != _originalVersion; }
        }
    }
}
