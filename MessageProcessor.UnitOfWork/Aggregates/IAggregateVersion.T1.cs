using System;

namespace YellowFlare.MessageProcessing.Aggregates
{
    /// <summary>
    /// Represent the version of a certain Aggregate.
    /// </summary>
    /// <typeparam name="TVersion">The specific Version type.</typeparam>
    public interface IAggregateVersion<TVersion> : IEquatable<TVersion>, IComparable<TVersion>
        where TVersion : struct, IAggregateVersion<TVersion>
    {
        /// <summary>
        /// Increments the current version to a new version.
        /// </summary>
        /// <returns>The incremented value.</returns>
        TVersion Increment();
    }
}
