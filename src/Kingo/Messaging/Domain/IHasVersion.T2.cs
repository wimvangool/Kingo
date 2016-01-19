using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents an Aggregate, following the definition of a Domain Driven Design.
    /// </summary>
    /// <typeparam name="TKey">Key or identifier of the Aggregate.</typeparam>
    /// <typeparam name="TVersion">Version of the aggregate.</typeparam>
    public interface IHasVersion<out TKey, out TVersion> : IHasKey<TKey>       
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
