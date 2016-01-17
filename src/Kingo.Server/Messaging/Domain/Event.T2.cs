using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents an event that is mapped to its contract.
    /// </summary>
    /// <typeparam name="TKey">Key-type of an aggregate.</typeparam>
    /// <typeparam name="TVersion">Version-type of an aggregate.</typeparam>
    public sealed class Event<TKey, TVersion> : SnapshotOrEvent<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        private readonly ITypeToContractMap _typeToContractMap;
        private readonly IVersionedObject<TKey, TVersion> _event;

        internal Event(ITypeToContractMap typeToContractMap, IVersionedObject<TKey, TVersion> @event)
        {
            if (typeToContractMap == null)
            {
                throw new ArgumentNullException("typeToContractMap");
            }
            if (@event == null)
            {
                throw new ArgumentNullException("event");
            }
            _typeToContractMap = typeToContractMap;
            _event = @event;
        }

        internal override ITypeToContractMap TypeToContractMap
        {
            get { return _typeToContractMap; }
        }

        internal override IVersionedObject<TKey, TVersion> VersionedObject
        {
            get { return _event; }
        }

        /// <summary>
        /// Returns the event that was published by the aggregate.
        /// </summary>
        public IVersionedObject<TKey, TVersion> Value
        {
            get { return _event; }
        }
    }
}
