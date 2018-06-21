using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a data-object or other object that can be used to restore an aggregate to its previous state.
    /// </summary>
    public interface IAggregateRootFactory
    {
        /// <summary>
        /// Restores an aggregate and returns its root.
        /// </summary>      
        /// <exception cref="NotSupportedException">
        /// The operation is not supported by this instance.
        /// </exception>  
        TAggregate RestoreAggregate<TAggregate>() where TAggregate : IAggregateRoot;
    }
}
