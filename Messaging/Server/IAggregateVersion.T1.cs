namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represent the version of a certain Aggregate.
    /// </summary>
    /// <typeparam name="TVersion">The specific Version type.</typeparam>
    public interface IAggregateVersion<TVersion> : IEquatable<TVersion>, IComparable<TVersion>
        where TVersion : struct, IAggregateVersion<TVersion>
    {
        /// <summary>
        /// Returns an incremented version of this version.
        /// </summary>
        /// <returns>The incremented version.</returns>
        TVersion Increment();
    }
}
