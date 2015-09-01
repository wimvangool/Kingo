using System;
using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.ComponentModel.Server
{
    /// <summary>
    /// A generic implementation of the <see cref="IEventBuffer"/> interface that buffers
    /// a single event until it is flushed to a <see cref="IDomainEventBus" />.
    /// </summary>
    /// <typeparam name="TMessage">Type of the event that is buffered.</typeparam>
    internal class EventBuffer<TMessage> : IEventBuffer where TMessage : class, IMessage<TMessage>
    {
        private readonly IDomainEventBus _eventBus;
        private readonly TMessage _domainEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBuffer{T}" /> class.
        /// </summary>
        /// <param name="eventBus">The event bus to which the event will be flushed.</param>
        /// <param name="domainEvent">The buffered event.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="eventBus"/> or <paramref name="domainEvent"/> is <c>null</c>.
        /// </exception>
        public EventBuffer(IDomainEventBus eventBus, TMessage domainEvent)
        {
            if (eventBus == null)
            {
                throw new ArgumentNullException("eventBus");
            }
            if (domainEvent == null)
            {
                throw new ArgumentNullException("domainEvent");
            }
            _eventBus = eventBus;
            _domainEvent = domainEvent;
        }

        /// <inheritdoc />
        public Task FlushAsync()
        {
            return _eventBus.PublishAsync(_domainEvent);
        }
    }
}
