using System;
using System.Transactions;
using static Kingo.Ensure;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a set of arguments for the <see cref="TransactionalQueue.Changed"/>-event.
    /// </summary>
    public sealed class TransactionalQueueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalQueueChangedEventArgs" /> class.
        /// </summary>
        /// <param name="transaction">The transaction that the changes were part of.</param>
        /// <param name="enqueueCount">The total number of items that were enqueued in the transaction.</param>
        /// <param name="dequeueCount">The total number of items that were dequeued in the transaction.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transaction"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="enqueueCount"/> or <paramref name="dequeueCount"/> is negative.
        /// </exception>
        public TransactionalQueueChangedEventArgs(Transaction transaction, int enqueueCount, int dequeueCount)
        {
            Transaction = IsNotNull(transaction, nameof(transaction));
            EnqueueCount = IsGreaterThanOrEqualTo(enqueueCount, 0, nameof(enqueueCount));
            DequeueCount = IsGreaterThanOrEqualTo(dequeueCount, 0, nameof(dequeueCount));
        }

        /// <summary>
        /// The transaction that the changes were part of.
        /// </summary>
        public Transaction Transaction
        {
            get;
        }

        /// <summary>
        /// The total number of items that were enqueued in the transaction.
        /// </summary>
        public int EnqueueCount
        {
            get;
        }

        /// <summary>
        /// The total number of items that were dequeued in the transaction.
        /// </summary>
        public int DequeueCount
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{nameof(EnqueueCount)} = {EnqueueCount}, {nameof(DequeueCount)} = {DequeueCount}";

        internal TransactionalQueueChangedEventArgs Append(TransactionalQueueChangedEventArgs other) =>
            new TransactionalQueueChangedEventArgs(Transaction, EnqueueCount + other.EnqueueCount, DequeueCount + other.DequeueCount);
    }
}
