namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents an Aggregate, following the definition of a Domain Driven Design.
    /// </summary>
    /// <typeparam name="TKey">Key or identifier of the Aggregate.</typeparam>
    /// <typeparam name="TVersion">Version of the aggregate.</typeparam>
    public interface IAggregate<out TKey, out TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>
    {
        /// <summary>
        /// Key or identifier of the Aggregate.
        /// </summary>
        TKey Key
        {
            get;
        }

        /// <summary>
        /// Version of the Aggregate.
        /// </summary>
        TVersion Version
        {
            get;
        }
    }
}
