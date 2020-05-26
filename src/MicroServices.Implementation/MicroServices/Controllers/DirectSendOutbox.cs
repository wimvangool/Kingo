using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a <see cref="MicroServiceBusOutbox"/> that simply forwards all messages that are to be sent
    /// immediately to another bus.
    /// </summary>
    public sealed class DirectSendOutbox : MicroServiceBusOutbox
    {
        #region [====== SenderClient ======]

        private sealed class SenderClient : MicroServiceBusClient
        {
            private readonly DirectSendOutbox _outbox;

            public SenderClient(DirectSendOutbox outbox)
            {
                _outbox = outbox;
            }

            public override Task SendAsync(IEnumerable<IMessage> messages) =>
                _outbox.Receiver.SendAsync(messages);
        }

        #endregion

        #region [====== ReceiverClient ======]

        private sealed class ReceiverClient : MicroServiceBusClient
        {
            private readonly DirectSendOutbox _outbox;

            public ReceiverClient(DirectSendOutbox outbox)
            {
                _outbox = outbox;
            }

            public override Task SendAsync(IEnumerable<IMessage> messages) =>
                _outbox.ServiceBus.SendAsync(messages);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectSendOutbox" /> class.
        /// </summary>
        /// <param name="serviceBus">The bus to which are messages are forwarded.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceBus"/> is <c>null</c>.
        /// </exception>
        public DirectSendOutbox(IMicroServiceBus serviceBus) :
            base(serviceBus) { }

        /// <inheritdoc />
        protected override Task<MicroServiceBusClient> CreateSenderAsync(CancellationToken token) =>
            Task.FromResult<MicroServiceBusClient>(new SenderClient(this));

        /// <inheritdoc />
        protected override Task<MicroServiceBusClient> CreateReceiverAsync(CancellationToken token) =>
            Task.FromResult<MicroServiceBusClient>(new ReceiverClient(this));
    }
}
