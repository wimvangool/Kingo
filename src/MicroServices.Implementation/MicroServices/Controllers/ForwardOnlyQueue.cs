using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a <see cref="StoreAndForwardQueue"/> that simply forwards all messages that are to be sent
    /// immediately to another bus.
    /// </summary>
    public sealed class ForwardOnlyQueue : StoreAndForwardQueue
    {
        #region [====== SenderProxy ======]

        private sealed class SenderProxy : MicroServiceBusProxy
        {
            private readonly ForwardOnlyQueue _queue;

            public SenderProxy(ForwardOnlyQueue queue)
            {
                _queue = queue;
            }

            public override Task SendAsync(IEnumerable<IMessage> messages) =>
                _queue.Receiver.SendAsync(messages);
        }

        #endregion

        #region [====== ReceiverProxy ======]

        private sealed class ReceiverProxy : MicroServiceBusProxy
        {
            private readonly ForwardOnlyQueue _queue;

            public ReceiverProxy(ForwardOnlyQueue queue)
            {
                _queue = queue;
            }

            public override Task SendAsync(IEnumerable<IMessage> messages) =>
                _queue.Bus.SendAsync(messages);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardOnlyQueue" /> class.
        /// </summary>
        /// <param name="bus">The bus to which are messages are forwarded.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="bus"/> is <c>null</c>.
        /// </exception>
        public ForwardOnlyQueue(IMicroServiceBus bus) :
            base(bus) { }

        /// <inheritdoc />
        protected override Task<MicroServiceBusProxy> StartSenderAsync(CancellationToken token) =>
            Task.FromResult<MicroServiceBusProxy>(new SenderProxy(this));

        /// <inheritdoc />
        protected override Task<MicroServiceBusProxy> StartReceiverAsync(CancellationToken token) =>
            Task.FromResult<MicroServiceBusProxy>(new ReceiverProxy(this));
    }
}
