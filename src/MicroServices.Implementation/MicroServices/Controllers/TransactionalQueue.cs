using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Kingo.Reflection;

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

            public abstract Task EnqueueAsync(IEnumerable<IMessage> messages, Transaction transaction);

            public abstract Task<IReadOnlyCollection<IMessage>> DequeueAsync(int batchSize, Transaction transaction);
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

            public override Task EnqueueAsync(IEnumerable<IMessage> messages, Transaction transaction) =>
                throw new NotImplementedException();

            public override Task<IReadOnlyCollection<IMessage>> DequeueAsync(int batchSize, Transaction transaction) =>
                throw new NotImplementedException();

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

            public override Task EnqueueAsync(IEnumerable<IMessage> messages, Transaction transaction) =>
                throw _queue.NewObjectDisposedException();

            public override Task<IReadOnlyCollection<IMessage>> DequeueAsync(int batchSize, Transaction transaction) =>
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

        #region [====== EnqueueAsync ======]

        /// <summary>
        /// Enqueues the specified <paramref name="messages"/> in this queue.
        /// </summary>
        /// <param name="messages">The messages to enqueue.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// This queue has already been disposed.
        /// </exception>
        public Task EnqueueAsync(IEnumerable<IMessage> messages) =>
            EnqueueAsync(messages, Transaction.Current);

        /// <summary>
        /// Enqueues the specified <paramref name="messages"/> in this queue as part of the specified <paramref name="transaction"/>.
        /// If <paramref name="transaction"/> is <c>null</c>, the queue will commit the operation immediately on completion.
        /// </summary>
        /// <param name="messages">The messages to enqueue.</param>
        /// <param name="transaction">Optional transaction that, if specified, this operation will enlist in.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// This queue has already been disposed.
        /// </exception>
        public Task EnqueueAsync(IEnumerable<IMessage> messages, Transaction transaction) =>
            _state.EnqueueAsync(messages, transaction);

        #endregion

        #region [====== DequeueAsync ======]

        /// <summary>
        /// The default (maximum) number of messages dequeued in a single <see cref="DequeueAsync(int, Transaction)"/>-operation.
        /// </summary>
        public const int DefaultBatchSize = 100;

        /// <summary>
        /// The minimum number of messages dequeued in a single <see cref="DequeueAsync(int, Transaction)"/>-operation.
        /// </summary>
        public const int MinimumBatchSize = 1;

        /// <summary>
        /// Dequeues a number of messages from this queue, where the maximum number of messages retrieved is limited
        /// by the specified <paramref name="batchSize" />.
        /// </summary>
        /// <param name="batchSize">The maximum number of messages to dequeue.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="batchSize"/> is less than the <see cref="MinimumBatchSize"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// This queue has already been disposed.
        /// </exception>
        public Task<IReadOnlyCollection<IMessage>> DequeueAsync(int batchSize = DefaultBatchSize) =>
            DequeueAsync(batchSize, Transaction.Current);

        /// <summary>
        /// Dequeues a number of messages from this queue, where the maximum number of messages retrieved is limited
        /// by the specified <paramref name="batchSize" />.
        /// </summary>
        /// <param name="batchSize">The maximum number of messages to dequeue.</param>
        /// <param name="transaction">Optional transaction that, if specified, this operation will enlist in.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="batchSize"/> is less than the <see cref="MinimumBatchSize"/>.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// This queue has already been disposed.
        /// </exception>
        public Task<IReadOnlyCollection<IMessage>> DequeueAsync(int batchSize, Transaction transaction) =>
            _state.DequeueAsync(batchSize, transaction);

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
