namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// Represents an Aggregate, following the definition of a Domain Driven Design.
    /// </summary>
    /// <typeparam name="TKey">Key or identifier of the Aggregate.</typeparam>
    /// <typeparam name="TVersion">Version of the aggregate.</typeparam>
    public interface IAggregateRoot<out TKey, out TVersion> : IAggregateRoot<TKey>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
    {        
        /// <summary>
        /// Version of the Aggregate.
        /// </summary>
        TVersion Version
        {
            get;
        }
    }
}
