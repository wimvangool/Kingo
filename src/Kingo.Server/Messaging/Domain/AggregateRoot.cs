using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents an aggregate root as defined by the principles of Domain Driven Design.
    /// </summary>
    public abstract class AggregateRoot : AggregateRoot<Guid, int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{T, S}" /> class.
        /// </summary>
        /// <param name="event">The event of that represents the creation of this aggregate.</param>        
        protected AggregateRoot(IHasKeyAndVersion<Guid, int> @event = null)
            : base(@event) { }

        /// <inheritdoc />
        protected override int NextVersion()
        {
            return Version + 1;
        }
    }
}
