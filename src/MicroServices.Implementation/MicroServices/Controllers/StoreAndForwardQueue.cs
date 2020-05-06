using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented, represents a queue that can be used to buffer messages in memory or on durable storage while
    /// the <see cref="Transaction"/> in which these messages were sent is still in progress. This mechanism allows
    /// the bus to discard the messages when the transaction is rolled back, or to forward them to another bus
    /// when the transaction is committed successfully.
    /// </summary>
    public abstract class StoreAndForwardQueue : MicroServiceBus
    {
        private readonly IMicroServiceBus _bus;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreAndForwardQueue" /> class.
        /// </summary>
        /// <param name="bus">The bus to which the messages will be forwarded.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bus"/> is <c>null</c>.
        /// </exception>
        protected StoreAndForwardQueue(IMicroServiceBus bus)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        /// <summary>
        /// The bus to which the messages will be forwarded.
        /// </summary>
        protected IMicroServiceBus Bus =>
            _bus;


    }
}
