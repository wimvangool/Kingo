namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// Represents an event that is raised or associated to a specific aggregate.
    /// </summary>
    /// <typeparam name="TKey">Type of the aggregate's key.</typeparam>
    /// <typeparam name="TVersion">Type of the aggregate's version.</typeparam>
    public interface IAggregateRootEvent<out TKey, out TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {
        /// <summary>
        /// Key of the associated aggregate.
        /// </summary>
        TKey AggregateKey
        {
            get;
        }

        /// <summary>
        /// Version of the aggregate right after this event was published.
        /// </summary>
        TVersion AggregateVersion
        {
            get;
        }
    }
}
