using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents either a snapshot or an event that is mapped to its contract.
    /// </summary>
    /// <typeparam name="TKey">Key-type of an aggregate.</typeparam>
    /// <typeparam name="TVersion">Version-type of an aggregate.</typeparam>
    public abstract class SnapshotOrEventToSave<TKey, TVersion> : IHasKeyAndVersion<TKey, TVersion>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        internal SnapshotOrEventToSave() { }

        TKey IHasKey<TKey>.Key
        {
            get { return VersionedObject.Key; }
        }                

        TVersion IHasKeyAndVersion<TKey, TVersion>.Version
        {
            get { return VersionedObject.Version; }
        }

        /// <summary>
        /// Returns the contract to which the snapshot or event is mapped.
        /// </summary>
        public string Contract
        {
            get { return TypeToContractMap.GetContract(VersionedObject.GetType()); }
        }

        internal abstract ITypeToContractMap TypeToContractMap
        {
            get;
        }

        internal abstract IHasKeyAndVersion<TKey, TVersion> VersionedObject
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("[{0} --> {1}]", VersionedObject.GetType().Name, Contract);
        }
    }
}
