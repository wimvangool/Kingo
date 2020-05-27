using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

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

            protected override Task SendAsync(IMessage[] messages) =>
                _outbox.Receiver.SendAsync(messages);

            protected override TransactionScope CreateTransactionScope() =>
                new TransactionScope(_outbox._scopeOption, _outbox._transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
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

            protected override Task SendAsync(IMessage[] messages) =>
                _outbox.ServiceBus.SendAsync(messages);

            protected override TransactionScope CreateTransactionScope()
            {
                // If the sender is configured to suppress any existing transaction, we must also
                // suppress it here. Otherwise, the receiver will accidentally create a new transaction.
                // In any other case, we just use the transaction that the sender just enlisted in or
                // created for itself.
                if (_outbox._scopeOption == TransactionScopeOption.Suppress)
                {
                    return new TransactionScope(_outbox._scopeOption, TransactionScopeAsyncFlowOption.Enabled);
                }
                return base.CreateTransactionScope();
            }
        }

        #endregion

        private readonly TransactionScopeOption _scopeOption;
        private readonly TransactionOptions _transactionOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectSendOutbox" /> class.
        /// </summary>
        /// <param name="serviceBus">The bus to which are messages are forwarded.</param>
        /// <param name="scopeOption">
        /// Indicates whether or not messages should be sent as part of an existing, new or no transaction at all.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceBus"/> is <c>null</c>.
        /// </exception>
        public DirectSendOutbox(IMicroServiceBus serviceBus, TransactionScopeOption scopeOption = TransactionScopeOption.Required) :
            this(serviceBus, scopeOption, new TransactionOptions()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectSendOutbox" /> class.
        /// </summary>
        /// <param name="serviceBus">The bus to which are messages are forwarded.</param>
        /// <param name="scopeOption">
        /// Indicates whether or not messages should be sent as part of an existing, new or no transaction at all.
        /// </param>
        /// <param name="transactionOptions">
        /// Specifies the timeout and isolation-level options of the transaction to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceBus"/> is <c>null</c>.
        /// </exception>
        public DirectSendOutbox(IMicroServiceBus serviceBus, TransactionScopeOption scopeOption, TransactionOptions transactionOptions) : base(serviceBus)
        {
            _scopeOption = scopeOption;
            _transactionOptions = transactionOptions;
        }

        /// <inheritdoc />
        protected override Task<MicroServiceBusClient> CreateSenderAsync(CancellationToken token) =>
            Task.FromResult<MicroServiceBusClient>(new SenderClient(this));

        /// <inheritdoc />
        protected override Task<MicroServiceBusClient> CreateReceiverAsync(CancellationToken token) =>
            Task.FromResult<MicroServiceBusClient>(new ReceiverClient(this));
    }
}
