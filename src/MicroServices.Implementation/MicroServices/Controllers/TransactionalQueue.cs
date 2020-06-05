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

            public abstract Task EnqueueAsync(IEnumerable<IMessage> messages);

            public abstract Task<IReadOnlyCollection<IMessage>> DequeueAsync(int batchSize);
        }

        #endregion

        #region [====== ActiveState ======]

        private sealed class ActiveState : State
        {
            private readonly TransactionalQueue _queue;

            public ActiveState(TransactionalQueue queue)
            {
                _queue = queue;
            }

            public override Task<int> CountAsync() =>
                _queue.CountItemsAsync();

            public override Task EnqueueAsync(IEnumerable<IMessage> messages) =>
                EnqueueAsync(_queue.Serialize(messages));

            private Task EnqueueAsync(IReadOnlyCollection<MicroServiceBusMessage> messages) =>
                messages.Count == 0 ? Task.CompletedTask : _queue.EnqueueAsync(messages);

            public override async Task<IReadOnlyCollection<IMessage>> DequeueAsync(int batchSize) =>
                _queue.Deserialize(await _queue.DequeueAsync(new BatchSize(batchSize)));

            protected override ValueTask DisposeAsync(DisposeContext context)
            {
                if (context != DisposeContext.Finalizer)
                {
                    Interlocked.Exchange(ref _queue._state, new DisposedState(_queue));
                }
                return base.DisposeAsync(context);
            }
        }

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

            public override Task EnqueueAsync(IEnumerable<IMessage> messages) =>
                throw _queue.NewObjectDisposedException();

            public override Task<IReadOnlyCollection<IMessage>> DequeueAsync(int batchSize) =>
                throw _queue.NewObjectDisposedException();

            protected override ValueTask DisposeAsync(DisposeContext context) =>
                default;
        }

        #endregion

        private State _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalQueue" /> class.
        /// </summary>
        protected TransactionalQueue()
        {
            _state = new ActiveState(this);
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()} ({_state})";

        #region [====== Serialization ======]

        /// <summary>
        /// Gets the serializer that is used to serialize and deserialize all messages that are stored in this queue.
        /// </summary>
        protected abstract IMessageSerializer Serializer
        {
            get;
        }

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
        /// </exception>
        /// <exception cref="SerializationException">
        /// At least one of the specified <paramref name="messages"/> could not be serialized.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// This queue has already been disposed.
        /// </exception>
        public Task EnqueueAsync(IEnumerable<IMessage> messages) =>
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
