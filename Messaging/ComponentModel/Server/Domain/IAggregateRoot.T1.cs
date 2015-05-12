namespace System.ComponentModel.Server.Domain
{
    /// <summary>
    /// Represents an Aggregate, following the definition of a Domain Driven Design.
    /// </summary>
    /// <typeparam name="TKey">Key or identifier of the Aggregate.</typeparam>
    public interface IAggregateRoot<out TKey> where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// Key or identifier of the Aggregate.
        /// </summary>
        TKey Key
        {
            get;
        }
    }
}
