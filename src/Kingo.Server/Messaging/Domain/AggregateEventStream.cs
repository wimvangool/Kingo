using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents an aggregate that is modeled as a stream of events and can also be restored as such.
    /// </summary>
    public abstract class AggregateEventStream : AggregateEventStream<Guid, int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot{T, S}" /> class.
        /// </summary>
        /// <param name="event">The event of that represents the creation of this aggregate.</param>        
        protected AggregateEventStream(IHasVersion<Guid, int> @event = null)
            : base(@event) { }

        /// <inheritdoc />
        protected override int NextVersion()
        {
            return Version + 1;
        }
    }
}
