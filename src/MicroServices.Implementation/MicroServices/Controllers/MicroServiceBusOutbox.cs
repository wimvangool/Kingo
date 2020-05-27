using System;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented, represents a conceptual outbox where messages that are produced by a <see cref="IMicroProcessor"/>
    /// can temporarily be stored before they are send out to another <see cref="IMicroServiceBus" />. The primary purpose of
    /// an outbox is to decouple the transaction/operation in which the messages are produced from the transaction/operation
    /// to physically transmit these messages over a network (i.e. to the <see cref="MicroServiceBusInbox"/>).
    /// </summary>
    public abstract class MicroServiceBusOutbox : MicroServiceBus
    {
        private readonly IMicroServiceBus _serviceBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusOutbox" /> class.
        /// </summary>
        /// <param name="serviceBus">The bus to which the messages will be forwarded.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceBus"/> is <c>null</c>.
        /// </exception>
        protected MicroServiceBusOutbox(IMicroServiceBus serviceBus) : base(MessageDirection.Output, MessageDirection.Output)
        {
            _serviceBus = serviceBus ?? throw new ArgumentNullException(nameof(serviceBus));
        }

        /// <summary>
        /// The bus to which the messages will be forwarded.
        /// </summary>
        protected IMicroServiceBus ServiceBus =>
            _serviceBus;
    }
}
