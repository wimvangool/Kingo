using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using static Kingo.Ensure;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented, represents a <see cref="IMicroServiceBus"/>-component that can either send or
    /// receive messages from a bus.
    /// </summary>
    public abstract class MicroServiceBusClient : AsyncDisposable, IMicroServiceBus
    {
        /// <summary>
        /// Sends the specified <paramref name="messages"/>. If this client represents a sender
        /// with respect to a service-bus, then this method will send the messages to te bus. If
        /// this client represents a receiver, this method will dispatch the messages as if they
        /// were received from the bus.
        /// </summary>
        /// <param name="messages">The messages to send.</param>
        /// <exception cref="InvalidOperationException">
        /// The client is currently unable to send any messages.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        public async Task SendAsync(IEnumerable<IMessage> messages)
        {
            using (var scope = CreateTransactionScope())
            {
                await SendAsync(IsNotNull(messages, nameof(messages)).ToArray());
                scope.Complete();
            }
        }

        /// <summary>
        /// Sends the specified <paramref name="messages"/>. If this client represents a sender
        /// with respect to a service-bus, then this method will send the messages to the bus. If
        /// this client represents a receiver, this method will dispatch the messages as if they
        /// were received from the bus.
        /// </summary>
        /// <param name="messages">The messages to send.</param>
        /// <exception cref="InvalidOperationException">
        /// The client is currently unable to send any messages.
        /// </exception>
        protected abstract Task SendAsync(IMessage[] messages);

        /// <summary>
        /// Creates and returns a new <see cref="TransactionScope"/> in which any messages will be sent.
        /// By default, this method returns a scope with <see cref="TransactionScopeOption.Required"/>,
        /// which will make sure the send-operation is executed in the existing transaction, if present,
        /// or in a new transaction, if the send-operation was not called as part of an existing transaction.
        /// </summary>
        /// <returns>A new <see cref="TransactionScope"/>.</returns>
        protected virtual TransactionScope CreateTransactionScope() =>
            new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
    }
}
