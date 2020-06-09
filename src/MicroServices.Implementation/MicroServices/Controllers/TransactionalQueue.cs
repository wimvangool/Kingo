using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Kingo.Collections.Generic;
using Kingo.Reflection;
using static Kingo.Ensure;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented, represents a message-queue that supports enqueue-ing and dequeue-ing messages
    /// as part of a <see cref="Transaction"/>.
    /// </summary>
    public abstract class TransactionalQueue : AsyncDisposable
    {
        #region [====== State ======]

        private abstract class State : AsyncDisposable
        {
            public override string ToString() =>
                GetType().FriendlyName().RemovePostfix(nameof(State));

            public abstract Task<int> CountAsync();

            public abstract Task<int> EnqueueAsync(IEnumerable<IMessage> messages);

            public abstract Task<IReadOnlyCollection<IMessage>> DequeueAsync(int batchSize);

            public abstract event EventHandler<TransactionalQueueChangedEventArgs> Changed;
        }

        #endregion

        #region [====== ActiveState ======]

        private sealed class ActiveState : State
        {
            private readonly TransactionalQueue _queue;
            private readonly Dictionary<string, TransactionalQueueChangedEventArgs> _changesPerTransaction;
            private EventHandler<TransactionalQueueChangedEventArgs> _changedEventHandlers;

            public ActiveState(TransactionalQueue queue)
            {
                _queue = queue;
                _changesPerTransaction = new Dictionary<string, TransactionalQueueChangedEventArgs>();
            }

            public override async Task<int> CountAsync()
            {
                using (var scope = NewTransactionScope())
                {
                    var count = await _queue.CountItemsAsync();
                    scope.Complete();
                    return count;
                }
            }

            public override Task<int> EnqueueAsync(IEnumerable<IMessage> messages) =>
                EnqueueAsync(_queue.Serialize(messages));

            private async Task<int> EnqueueAsync(IReadOnlyCollection<MicroServiceBusMessage> messages)
            {
                if (messages.Count == 0)
                {
                    return 0;
                }
                using (var scope = NewTransactionScope())
                {
                    await EnqueueAsync(messages, Transaction.Current);
                    scope.Complete();
                }
                return messages.Count;
            }

            private async Task EnqueueAsync(IReadOnlyCollection<MicroServiceBusMessage> messages, Transaction transaction)
            {
                await _queue.EnqueueAsync(messages);

                OnChanged(new TransactionalQueueChangedEventArgs(transaction, messages.Count, 0));
            }

            public override Task<IReadOnlyCollection<IMessage>> DequeueAsync(int batchSize) =>
                DequeueAsync(new BatchSize(batchSize));

            private async Task<IReadOnlyCollection<IMessage>> DequeueAsync(BatchSize batchSize)
            {
                using (var scope = NewTransactionScope())
                {
                    var messages = await DequeueAsync(batchSize, Transaction.Current);
                    scope.Complete();
                    return messages;
                }
            }

            private async Task<IReadOnlyCollection<IMessage>> DequeueAsync(BatchSize batchSize, Transaction transaction)
            {
                var messages = _queue.Deserialize(await _queue.DequeueAsync(batchSize));
                if (messages.Count > 0)
                {
                    OnChanged(new TransactionalQueueChangedEventArgs(transaction, 0, messages.Count));
                }
                return messages;
            }

            public override event EventHandler<TransactionalQueueChangedEventArgs> Changed
            {
                add
                {
                    lock (this)
                    {
                        _changedEventHandlers += value;
                    }
                }
                remove
                {
                    lock (this)
                    {
                        _changedEventHandlers -= value;
                    }
                }
            }

            private void OnChanged(TransactionalQueueChangedEventArgs e) =>
                OnChanged(e, e.Transaction.TransactionInformation.LocalIdentifier);

            private void OnChanged(TransactionalQueueChangedEventArgs e, string transactionId)
            {
                lock (_changesPerTransaction)
                {
                    if (_changesPerTransaction.TryGetValue(transactionId, out var changes))
                    {
                        _changesPerTransaction[transactionId] = changes.Append(e);
                    }
                    else
                    {
                        _changesPerTransaction[transactionId] = e;

                        e.Transaction.TransactionCompleted += HandleTransactionCompleted;
                    }
                }
            }

            private void HandleTransactionCompleted(object sender, TransactionEventArgs e)
            {
               if (TryGetChanges(e.Transaction.TransactionInformation.LocalIdentifier, out var changes))
               {
                   if (e.Transaction.TransactionInformation.Status == TransactionStatus.Committed)
                   {
                       _changedEventHandlers.Raise(_queue, changes);
                   }
               }
            }

            private bool TryGetChanges(string transactionId, out TransactionalQueueChangedEventArgs changes)
            {
                lock (_changesPerTransaction)
                {
                    if (_changesPerTransaction.TryGetValue(transactionId, out changes))
                    {
                        _changesPerTransaction.Remove(transactionId);
                        return true;
                    }
                    return false;
                }
            }

            protected override ValueTask DisposeAsync(DisposeContext context)
            {
                if (context != DisposeContext.Finalizer)
                {
                    Interlocked.Exchange(ref _queue._state, new DisposedState(_queue));
                }
                return base.DisposeAsync(context);
            }
        }

        private static TransactionScope NewTransactionScope() =>
            new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        #endregion

        #region [====== DisposedState ======]

        private sealed class DisposedState : State
        {
            private readonly TransactionalQueue _queue;

            public DisposedState(TransactionalQueue queue)
            {
                _queue = queue;
            }

            public override Task<int> CountAsync() =>
                throw _queue.NewObjectDisposedException();

            public override Task<int> EnqueueAsync(IEnumerable<IMessage> messages) =>
                throw _queue.NewObjectDisposedException();

            public override Task<IReadOnlyCollection<IMessage>> DequeueAsync(int batchSize) =>
                throw _queue.NewObjectDisposedException();

            public override event EventHandler<TransactionalQueueChangedEventArgs> Changed
            {
                add => throw _queue.NewObjectDisposedException();
                remove => throw _queue.NewObjectDisposedException();
            }

            protected override ValueTask DisposeAsync(DisposeContext context) =>
                default;
        }

        #endregion

        private readonly IMessageSerializer _serializer;
        private State _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalQueue" /> class.
        /// </summary>
        /// <param name="serializer">The serializer that is used to serialize and deserialize all messages that are stored in this queue.</param>
        protected TransactionalQueue(IMessageSerializer serializer = null)
        {
            _serializer = serializer ?? new MessageSerializer();
            _state = new ActiveState(this);
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()} ({_state})";

        #region [====== Serialization ======]

        /// <summary>
        /// Gets the serializer that is used to serialize and deserialize all messages that are stored in this queue.
        /// </summary>
        protected virtual IMessageSerializer Serializer =>
            _serializer;

        private IReadOnlyCollection<MicroServiceBusMessage> Serialize(IEnumerable<IMessage> messages) =>
            IsNotNull(messages, nameof(messages)).WhereNotNull().Select(Serialize).ToArray();

        private MicroServiceBusMessage Serialize(IMessage message) =>
            new MicroServiceBusMessage(Serializer.Serialize(message), message.Kind);

        private IReadOnlyCollection<IMessage> Deserialize(IEnumerable<MicroServiceBusMessage> messages) =>
            IsNotNull(messages, nameof(messages)).WhereNotNull().Select(Deserialize).ToArray();

        private IMessage Deserialize(MicroServiceBusMessage message) =>
            Serializer.DeserializeOutput(message, message.Kind);

        #endregion

        #region [====== CountAsync ======]

        /// <summary>
        /// Counts all messages currently stored inside the queue.
        /// </summary>
        /// <returns>The number of messages currently stored inside the queue.</returns>
        /// <exception cref="ObjectDisposedException">
        /// This queue has already been disposed.
        /// </exception>
        public Task<int> CountAsync() =>
            _state.CountAsync();

        /// <summary>
        /// Counts all messages currently stored inside the queue.
        /// </summary>
        /// <returns>The number of messages currently stored inside the queue.</returns>
        protected abstract Task<int> CountItemsAsync();

        #endregion

        #region [====== EnqueueAsync ======]

        /// <summary>
        /// Enqueues the specified <paramref name="messages"/> in this queue.
        /// </summary>
        /// <param name="messages">The messages to enqueue.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// <returns>The number of messages that were enqueued.</returns>
        /// </exception>
        /// <exception cref="SerializationException">
        /// At least one of the specified <paramref name="messages"/> could not be serialized.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// This queue has already been disposed.
        /// </exception>
        public Task<int> EnqueueAsync(IEnumerable<IMessage> messages) =>
            _state.EnqueueAsync(messages);

        /// <summary>
        /// Enqueues the specified <paramref name="messages"/> in this queue.
        /// </summary>
        /// <param name="messages">The messages to enqueue.</param>
        protected abstract Task EnqueueAsync(IReadOnlyCollection<MicroServiceBusMessage> messages);

        #endregion

        #region [====== DequeueAsync ======]

        /// <summary>
        /// Dequeues a number of messages from this queue, where the maximum number of messages retrieved is limited
        /// by the specified <paramref name="batchSize" />.
        /// </summary>
        /// <param name="batchSize">The maximum number of messages to dequeue.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="batchSize"/> is less than the <see cref="BatchSize.MinValue"/>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// At least one of the messages dequeued in the operation could not be deserialized.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// This queue has already been disposed.
        /// </exception>
        public Task<IReadOnlyCollection<IMessage>> DequeueAsync(int batchSize = BatchSize.DefaultValue) =>
            _state.DequeueAsync(batchSize);

        /// <summary>
        /// Dequeues a number of messages from this queue, where the maximum number of messages retrieved is limited
        /// by the specified <paramref name="batchSize" />.
        /// </summary>
        /// <param name="batchSize">The maximum number of messages to dequeue.</param>
        protected abstract Task<IEnumerable<MicroServiceBusMessage>> DequeueAsync(BatchSize batchSize);

        #endregion

        #region [====== Changed (Event) ======]

        /// <summary>
        /// Represents an event that is raised when one or more <see cref="EnqueueAsync"/> or <see cref="DequeueAsync"/>
        /// operations have been executed on this queue as part of a specific transaction.
        /// </summary>
        public event EventHandler<TransactionalQueueChangedEventArgs> Changed
        {
            add => _state.Changed += value;
            remove => _state.Changed -= value;
        }

        #endregion

        #region [====== DisposeAsync ======]

        /// <inheritdoc />
        protected override async ValueTask DisposeAsync(DisposeContext context)
        {
            if (context != DisposeContext.Finalizer)
            {
                await _state.DisposeAsync();
            }
            await base.DisposeAsync(context);
        }

        #endregion
    }
}
