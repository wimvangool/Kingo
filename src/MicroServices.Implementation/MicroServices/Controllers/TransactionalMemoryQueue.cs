using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a <see cref="StoreAndForwardQueue"/> that buffers all messages in memory before
    /// forwarding them to another bus.
    /// </summary>
    public sealed class TransactionalMemoryQueue : StoreAndForwardQueue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalMemoryQueue" /> class.
        /// </summary>
        /// <param name="bus">The bus to which are messages are forwarded.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bus"/> is <c>null</c>.
        /// </exception>
        public TransactionalMemoryQueue(IMicroServiceBus bus) :
            base(bus) { }

        /// <inheritdoc />
        protected override Task<MicroServiceBusProxy> StartSenderAsync(CancellationToken token) =>
            throw new NotImplementedException();

        /// <inheritdoc />
        protected override Task<MicroServiceBusProxy> StartReceiverAsync(CancellationToken token) =>
            throw new NotImplementedException();
    }
}
