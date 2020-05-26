using System;
using System.Transactions;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented, represents a conceptual outbox where messages that are produced by a <see cref="IMicroProcessor"/>
    /// can temporarily be stored before they are send out to another <see cref="IMicroServiceBus" />. An outbox can be useful
    /// to decouple the transaction/operation in which the messages are produced from the task to actually send these messages
    /// to a service-bus.
    /// </summary>
    public abstract class MicroServiceBusOutbox : MicroServiceBusBase
    {
        private readonly IMicroServiceBus _serviceBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusOutbox" /> class.
        /// </summary>
        /// <param name="serviceBus">The bus to which the messages will be forwarded.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceBus"/> is <c>null</c>.
        /// </exception>
        protected MicroServiceBusOutbox(IMicroServiceBus serviceBus)
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
