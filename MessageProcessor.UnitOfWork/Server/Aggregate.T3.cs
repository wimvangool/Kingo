using System;

namespace YellowFlare.MessageProcessing.Server
{
    internal sealed class Aggregate<TKey, TVersion, TValue>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IAggregateVersion<TVersion>
        where TValue : class, IAggregate<TKey, TVersion>
    {
        private readonly TVersion _originalVersion;
        private readonly TValue _value;

        public Aggregate(TValue value)
        {
            _originalVersion = value.Version;
            _value = value;
        }

        public TVersion OriginalVersion
        {
            get { return _originalVersion; }
        }

        public TValue Value
        {
            get { return _value; }
        }

        public bool HasBeenUpdated
        {
            get { return !_value.Version.Equals(_originalVersion); }
        }
    }
}
