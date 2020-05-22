using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a unique identifier that is assigned to a specific <see cref="MicroProcessorTestOperationInfo" />.
    /// </summary>
    [Serializable]
    public struct MicroProcessorTestOperationId : IEquatable<MicroProcessorTestOperationId>
    {
        /// <summary>
        /// Represents a null or empty identifier.
        /// </summary>
        public static readonly MicroProcessorTestOperationId Empty = new MicroProcessorTestOperationId();

        private readonly Guid _id;

        private MicroProcessorTestOperationId(Guid id)
        {
            _id = id;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            obj is MicroProcessorTestOperationId other && Equals(other);

        /// <inheritdoc />
        public bool Equals(MicroProcessorTestOperationId other) =>
            _id.Equals(other._id);

        /// <inheritdoc />
        public override int GetHashCode() =>
            _id.GetHashCode();

        /// <summary>
        /// Generates and returns a new operation id.
        /// </summary>
        /// <returns>A new unique operation id.</returns>
        public static MicroProcessorTestOperationId NewOperationId() =>
            new MicroProcessorTestOperationId(Guid.NewGuid());
    }
}
