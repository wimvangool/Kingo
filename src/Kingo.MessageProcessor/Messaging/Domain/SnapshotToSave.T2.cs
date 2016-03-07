using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a snapshot that is mapped to its contract.
    /// </summary>
    /// <typeparam name="TKey">Key-type of an aggregate.</typeparam>
    /// <typeparam name="TVersion">Version-type of an aggregate.</typeparam>
    public sealed class SnapshotToSave<TKey, TVersion> : SnapshotOrEventToSave<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        private readonly ITypeToContractMap _typeToContractMap;
        private readonly IMemento<TKey, TVersion> _memento;

        internal SnapshotToSave(ITypeToContractMap typeToContractMap, IMemento<TKey, TVersion> memento)
        {
            if (typeToContractMap == null)
            {
                throw new ArgumentNullException(nameof(typeToContractMap));
            }
            if (memento == null)
            {
                throw new ArgumentNullException(nameof(memento));
            }
            _typeToContractMap = typeToContractMap;
            _memento = memento;
        }

        internal override ITypeToContractMap TypeToContractMap
        {
            get { return _typeToContractMap; }
        }

        internal override IHasKeyAndVersion<TKey, TVersion> VersionedObject
        {
            get { return _memento; }
        }

        /// <summary>
        /// Returns the snapshot of the aggregate.
        /// </summary>
        public IMemento<TKey, TVersion> Value
        {
            get { return _memento; }
        }
    }
}
