using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a snapshot that is mapped to its contract.
    /// </summary>
    /// <typeparam name="TKey">Key-type of an aggregate.</typeparam>
    /// <typeparam name="TVersion">Version-type of an aggregate.</typeparam>
    public sealed class Snapshot<TKey, TVersion> : SnapshotOrEvent<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        private readonly ITypeToContractMap _typeToContractMap;
        private readonly ISnapshot<TKey, TVersion> _snapshot;

        internal Snapshot(ITypeToContractMap typeToContractMap, ISnapshot<TKey, TVersion> snapshot)
        {
            if (typeToContractMap == null)
            {
                throw new ArgumentNullException("typeToContractMap");
            }
            if (snapshot == null)
            {
                throw new ArgumentNullException("snapshot");
            }
            _typeToContractMap = typeToContractMap;
            _snapshot = snapshot;
        }

        internal override ITypeToContractMap TypeToContractMap
        {
            get { return _typeToContractMap; }
        }

        internal override IHasKeyAndVersion<TKey, TVersion> VersionedObject
        {
            get { return _snapshot; }
        }

        /// <summary>
        /// Returns the snapshot of the aggregate.
        /// </summary>
        public ISnapshot<TKey, TVersion> Value
        {
            get { return _snapshot; }
        }
    }
}
