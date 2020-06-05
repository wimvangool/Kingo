using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Kingo.Reflection;
using static Kingo.Ensure;
using Timer = System.Timers.Timer;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents a message-queue that supports enqueue-ing and dequeue-ing messages
    /// as part of a <see cref="Transaction"/> by storing them in memory.
    /// </summary>
    public sealed class TransactionalMemoryQueue : TransactionalQueue
    {
        #region [====== TransactionOperationManager ======]

        // The TransactionOperationManager is responsible for executing the operations that are part
        // of one specific transaction. It makes sure every operation is executed on the thread
        // that is strictly associated with the transaction it manages. This is required to make sure
        // the traditional locks with thread-affinity work as expected.
        private sealed class TransactionOperationManager : AsyncDisposable, ISinglePhaseNotification
        {
            private readonly TransactionalMemoryQueue _queue;
            private readonly Transaction _transaction;
            private readonly Thread _transactionThread;
            private readonly ManualResetEventSlim _operationWaitHandle;
            private readonly CancellationTokenSource _operationWaitHandleTokenSource;
            private readonly ConcurrentQueue<TransactionOperation> _operationsToExecute;
            private readonly List<TransactionOperation> _operationsToComplete;
            private TransactionCompletionOperation _transactionCompletionOperation;

            public TransactionOperationManager(TransactionalMemoryQueue queue, Transaction transaction)
            {
                _queue = queue;
                _transaction = transaction;
                _transactionThread = new Thread(ExecuteTransaction)
                {
                    Name = $"Transaction (LocalId = {transaction.TransactionInformation.LocalIdentifier}, IsolationLevel = {transaction.IsolationLevel})"
                };
                _operationWaitHandle = new ManualResetEventSlim();
                _operationWaitHandleTokenSource = new CancellationTokenSource();
                _operationsToExecute = new ConcurrentQueue<TransactionOperation>();
                _operationsToComplete = new List<TransactionOperation>();
                _transactionCompletionOperation = new NullOperation();
            }

            public TransactionOperationManager Start()
            {
                _transaction.EnlistVolatile(this, EnlistmentOptions.None);
                _transactionThread.Start();
                return this;
            }

            // This method, executed by any client-thread, schedules the specified operation to be
            // executed by this manager and then waits for the result to become available. It essentially
            // plays the role of the producer in the producer/consumer-pattern implemented by the manager.
            public Task<IReadOnlyCollection<MicroServiceBusMessage>> ExecuteAsync(TransactionOperation operation)
            {
                // As soon as the operation is added, the wait-handle is signaled so that it can be executed.
                _operationsToExecute.Enqueue(operation);
                _operationWaitHandle.Set();

                return operation.WaitForResultAsync(_operationWaitHandleTokenSource.Token);
            }

            // This method, executed by the thread associated with the transaction, is responsible for
            // consuming (i.e. executing) all operations added to the queue by its Async-counterpart.
            // When done, it will complete the transaction and clean up the necessary resources.
            private void ExecuteTransaction()
            {
                // In phase one, all operations that are part of this transaction are executed.
                // Phase one ends when all operations have been executed and the transaction has opted
                // to complete.
                while (WaitForNextOperationsToExecute())
                {
                    while (_operationsToExecute.TryDequeue(out var operation))
                    {
                        _operationsToComplete.Add(operation.Execute(_queue, _transaction.IsolationLevel));
                    }
                }

                // In phase two, all operations are committed or rolled back, depending on the outcome. This operation
                // will make sure all locks held by the transaction are released. After this step, the transaction
                // is logically completed and the manager can clean up after itself.
                _transactionCompletionOperation.Execute(_operationsToComplete);
            }

            private bool WaitForNextOperationsToExecute()
            {
                // The worker-thread is made to wait for the next operation to come in,
                // in which case the wait-handle will be signaled by the thread that has added
                // the next operation. If, however, the cancellation-token is signaled before
                // that, the worker-thread can stop executing new TransactionOperations.
                try
                {
                    _operationWaitHandle.Wait(_operationWaitHandleTokenSource.Token);
                    _operationWaitHandle.Reset();
                    return true;
                }
                catch (OperationCanceledException)
                {
                    return false;
                }
            }

            void ISinglePhaseNotification.SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment) =>
                Execute(new SinglePhaseCommitOperation(_queue, _transaction.IsolationLevel, singlePhaseEnlistment));

            void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment) =>
                preparingEnlistment.Prepared();

            void IEnlistmentNotification.InDoubt(Enlistment enlistment) =>
                Execute(new InDoubtOperation(_queue, _transaction.IsolationLevel, enlistment));

            void IEnlistmentNotification.Commit(Enlistment enlistment) =>
                Execute(new CommitOperation(_queue, _transaction.IsolationLevel, enlistment));

            void IEnlistmentNotification.Rollback(Enlistment enlistment) =>
                Execute(new RollbackOperation(_queue, _transaction.IsolationLevel, enlistment));

            private void Execute(TransactionCompletionOperation operation)
            {
                Interlocked.Exchange(ref _transactionCompletionOperation, operation).Dispose();

                _operationWaitHandleTokenSource.Cancel();
                _transactionCompletionOperation.WaitForCompletion(_queue._lockTimeout);
            }

            protected override async ValueTask DisposeAsync(DisposeContext context)
            {
                if (context != DisposeContext.Finalizer)
                {
                    _operationWaitHandleTokenSource.Dispose();
                    _operationWaitHandle.Dispose();

                    await _transactionCompletionOperation.DisposeAsync();
                }
                await base.DisposeAsync(context);
            }
        }

        #endregion

        #region [===== TransactionCompletionOperations ======]

        private abstract class TransactionCompletionOperation : AsyncDisposable
        {
            private readonly List<TransactionOperation> _completedOperations;
            private readonly ManualResetEventSlim _waitHandle;
            private Exception _exception;

            protected TransactionCompletionOperation()
            {
                _completedOperations = new List<TransactionOperation>();
                _waitHandle = new ManualResetEventSlim();
            }

            public virtual void Execute(IEnumerable<TransactionOperation> operationsToComplete)
            {
                try
                {
                    foreach (var operation in operationsToComplete)
                    {
                        _completedOperations.Add(Complete(operation));
                    }
                }
                catch (Exception exception)
                {
                    _exception = exception;
                }
                finally
                {
                    _waitHandle.Set();
                }
            }

            protected abstract TransactionOperation Complete(TransactionOperation operation);

            public void WaitForCompletion(TimeSpan timeout)
            {
                if (_waitHandle.Wait(timeout))
                {
                    if (_exception == null)
                    {
                        return;
                    }
                    throw NewCompletionFailedException(_exception);
                }
                throw NewLockTimeoutException(timeout);
            }

            protected override async ValueTask DisposeAsync(DisposeContext context)
            {
                if (context != DisposeContext.Finalizer)
                {
                    foreach (var operation in _completedOperations)
                    {
                        await operation.DisposeAsync();
                    }
                    _waitHandle.Dispose();
                }
                await base.DisposeAsync(context);
            }

            private static Exception NewCompletionFailedException(Exception exception) =>
                new AggregateException(ExceptionMessages.TransactionalMemoryQueue_CompletionFailed, exception);
        }

        private sealed class NullOperation : TransactionCompletionOperation
        {
            protected override TransactionOperation Complete(TransactionOperation operation) =>
                throw NewTransactionCannotBeCompletedException();

            private static Exception NewTransactionCannotBeCompletedException() =>
                new InvalidOperationException(ExceptionMessages.TransactionalMemoryQueue_CannotCompleteTransaction);
        }

        private sealed class SinglePhaseCommitOperation : TransactionCompletionOperation
        {
            private readonly TransactionalMemoryQueue _queue;
            private readonly IsolationLevel _isolationLevel;
            private readonly SinglePhaseEnlistment _enlistment;

            public SinglePhaseCommitOperation(TransactionalMemoryQueue queue, IsolationLevel isolationLevel, SinglePhaseEnlistment enlistment)
            {
                _queue = queue;
                _isolationLevel = isolationLevel;
                _enlistment = enlistment;
            }

            public override void Execute(IEnumerable<TransactionOperation> operationsToComplete)
            {
                try
                {
                    base.Execute(operationsToComplete);

                    _enlistment.Committed();
                }
                catch (Exception exception)
                {
                    _enlistment.Aborted(exception);
                }
            }

            protected override TransactionOperation Complete(TransactionOperation operation)
            {
                operation.Commit(_queue, _isolationLevel);
                return operation;
            }
        }

        private abstract class TwoPhaseCommitOperation : TransactionCompletionOperation
        {
            private readonly TransactionalMemoryQueue _queue;
            private readonly IsolationLevel _isolationLevel;
            private readonly Enlistment _enlistment;

            protected TwoPhaseCommitOperation(TransactionalMemoryQueue queue, IsolationLevel isolationLevel, Enlistment enlistment)
            {
                _queue = queue;
                _isolationLevel = isolationLevel;
                _enlistment = enlistment;
            }

            public override void Execute(IEnumerable<TransactionOperation> operationsToComplete)
            {
                try
                {
                    base.Execute(operationsToComplete);
                }
                finally
                {
                    _enlistment.Done();
                }
            }

            protected override TransactionOperation Complete(TransactionOperation operation) =>
                Complete(operation, _queue, _isolationLevel);

            protected abstract TransactionOperation Complete(TransactionOperation operation, TransactionalMemoryQueue queue, IsolationLevel isolationLevel);
        }

        private sealed class InDoubtOperation : TwoPhaseCommitOperation
        {
            public InDoubtOperation(TransactionalMemoryQueue queue, IsolationLevel isolationLevel, Enlistment enlistment) :
                base(queue, isolationLevel, enlistment) { }

            // Transactions in Doubt are treated as failed transactions by the queue. Hence, we reverse the operations to complete
            // in order to rollback their changes from last to first.
            public override void Execute(IEnumerable<TransactionOperation> operationsToComplete) =>
                base.Execute(operationsToComplete.Reverse());

            protected override TransactionOperation Complete(TransactionOperation operation, TransactionalMemoryQueue queue, IsolationLevel isolationLevel)
            {
                operation.InDoubt(queue, isolationLevel);
                return operation;
            }
        }

        private sealed class CommitOperation : TwoPhaseCommitOperation
        {
            public CommitOperation(TransactionalMemoryQueue queue, IsolationLevel isolationLevel, Enlistment enlistment) :
                base(queue, isolationLevel, enlistment) { }

            protected override TransactionOperation Complete(TransactionOperation operation, TransactionalMemoryQueue queue, IsolationLevel isolationLevel)
            {
                operation.Commit(queue, isolationLevel);
                return operation;
            }
        }

        private sealed class RollbackOperation : TwoPhaseCommitOperation
        {
            public RollbackOperation(TransactionalMemoryQueue queue, IsolationLevel isolationLevel, Enlistment enlistment) :
                base(queue, isolationLevel, enlistment) { }

            // Transactions that failed and need to be rolled back should rollback their changes in reverse order.
            public override void Execute(IEnumerable<TransactionOperation> operationsToComplete) =>
                base.Execute(operationsToComplete.Reverse());

            protected override TransactionOperation Complete(TransactionOperation operation, TransactionalMemoryQueue queue, IsolationLevel isolationLevel)
            {
                operation.Rollback(queue, isolationLevel);
                return operation;
            }
        }

        #endregion

        #region [====== TransactionOperation ======]

        // A TransactionOperation represents a single operation on the queue that belongs to one specific transaction.
        // This can be a read-operation (SELECT) or write-operation (INSERT, UPDATE OR DELETE).
        private abstract class TransactionOperation : AsyncDisposable
        {
            private readonly ManualResetEventSlim _waitHandle;
            private readonly List<QueueItem> _items;
            private Exception _exception;

            protected TransactionOperation()
            {
                _waitHandle = new ManualResetEventSlim();
                _items = new List<QueueItem>();
            }

            public TransactionOperation Execute(TransactionalMemoryQueue queue, IsolationLevel isolationLevel)
            {
                try
                {
                    Execute(queue, _items, isolationLevel);
                    return this;
                }
                catch (Exception exception)
                {
                    _exception = exception;
                    return this;
                }
                finally
                {
                    _waitHandle.Set();
                }
            }

            // When the operation is executed, it adds all items it touched to the specified items-collection. This collection
            // is later supplied to one of the complete-operations to allow the operation to release any locks it still holds
            // on those items.
            protected abstract void Execute(TransactionalMemoryQueue queue, ICollection<QueueItem> items, IsolationLevel isolationLevel);

            public Task<IReadOnlyCollection<MicroServiceBusMessage>> WaitForResultAsync(CancellationToken token)
            {
                try
                {
                    _waitHandle.Wait(token);

                    if (_exception == null)
                    {
                        return Task.FromResult<IReadOnlyCollection<MicroServiceBusMessage>>(_items.Select(item => item.Message).ToArray());
                    }
                    return Task.FromException<IReadOnlyCollection<MicroServiceBusMessage>>(_exception);
                }
                catch (OperationCanceledException exception)
                {
                    return Task.FromCanceled<IReadOnlyCollection<MicroServiceBusMessage>>(exception.CancellationToken);
                }
                catch (Exception exception)
                {
                    return Task.FromException<IReadOnlyCollection<MicroServiceBusMessage>>(exception);
                }
            }

            public void InDoubt(TransactionalMemoryQueue queue, IsolationLevel isolationLevel) =>
                InDoubt(queue, _items, isolationLevel);

            protected abstract void InDoubt(TransactionalMemoryQueue queue, IEnumerable<QueueItem> items, IsolationLevel isolationLevel);

            public void Commit(TransactionalMemoryQueue queue, IsolationLevel isolationLevel) =>
                Commit(queue, _items, isolationLevel);

            protected abstract void Commit(TransactionalMemoryQueue queue, IEnumerable<QueueItem> items, IsolationLevel isolationLevel);

            public void Rollback(TransactionalMemoryQueue queue, IsolationLevel isolationLevel) =>
                Rollback(queue, _items, isolationLevel);

            protected abstract void Rollback(TransactionalMemoryQueue queue, IEnumerable<QueueItem> items, IsolationLevel isolationLevel);

            protected override ValueTask DisposeAsync(DisposeContext context)
            {
                if (context != DisposeContext.Finalizer)
                {
                    _waitHandle.Dispose();
                }
                return base.DisposeAsync(context);
            }
        }

        #endregion

        #region [====== SelectOperation ======]

        // The SelectOperation simply iterates over the entire queue, acquiring the locks as needed.
        private sealed class SelectOperation : TransactionOperation
        {
            protected override void Execute(TransactionalMemoryQueue queue, ICollection<QueueItem> items, IsolationLevel isolationLevel)
            {
                switch (isolationLevel)
                {
                    case IsolationLevel.Serializable:
                        ExecuteSerializable(queue, items);
                        return;
                    case IsolationLevel.RepeatableRead:
                        ExecuteRepeatableRead(queue, items);
                        return;
                    case IsolationLevel.ReadCommitted:
                        ExecuteReadCommitted(queue, items);
                        return;
                    case IsolationLevel.ReadUncommitted:
                        ExecuteReadUncommitted(queue, items);
                        return;
                    default:
                        throw NewIsolationLevelNotSupportedException(isolationLevel);
                }
            }

            // In isolation-level Serialize, the select-operation attempts to obtain a read-lock on the entire
            // queue to prevent other transactions to insert or delete items while the transaction is still in
            // progress. This prevents so-called phantom-reads (newly inserted records) when the select-operation
            // is executed more than once. The lock is released when the transaction ends.
            private static void ExecuteSerializable(TransactionalMemoryQueue queue, ICollection<QueueItem> items)
            {
                if (queue._itemsLock.TryEnterUpgradeableReadLock(queue._lockTimeout))
                {
                    ExecuteRepeatableRead(queue, items);
                    return;
                }
                throw NewLockTimeoutException(queue._lockTimeout);
            }

            // In isolation-level RepeatableRead (or higher), the select-operation attempts to obtain upgradeable
            // read-locks on each item as they are enumerated. This prevents other transactions from modifying
            // (i.e. deleting) the items after they've been read by this transaction, ensuring the same results
            // when the read-operation is executed more than once (hence repeatable-read). The locks are released
            // when the transaction ends.
            private static void ExecuteRepeatableRead(TransactionalMemoryQueue queue, ICollection<QueueItem> items)
            {
                using (var enumerator = new QueueItemEnumerator(queue))
                {
                    while (enumerator.MoveNext())
                    {
                        var item = enumerator.Current;
                        if (item.EnterUpgradeableReadLock(queue._lockTimeout))
                        {
                            Select(item, items);
                        }
                    }
                }
            }

            // In isolation-level ReadCommitted, the select-operation attempts to obtain a read-lock on each item
            // as they are enumerated. When the read-lock is obtained, it is implied that the item is in a (non-deleted)
            // committed state. The read-lock is immediately releases, which allows other transactions to obtain upgradeable
            // or write-locks on it, improving performance and decreasing the risk of dead-locks. The price for this is less
            // consistency: a second select-operation may yield different results.
            private static void ExecuteReadCommitted(TransactionalMemoryQueue queue, ICollection<QueueItem> items)
            {
                using (var enumerator = new QueueItemEnumerator(queue))
                {
                    while (enumerator.MoveNext())
                    {
                        var item = enumerator.Current;
                        if (item.EnterReadLock(queue._lockTimeout))
                        {
                            Select(item, items);

                            item.ExitReadLock();
                        }
                    }
                }
            }

            // In isolation-level ReadUncommitted, no locks are obtained on any item and the IsCommitted-flag is ignored
            // while iterating the queue. This means that all items that are currently marked as non-deleted, committed or not,
            // will be returned in the result-set. This setting provides maximum performance, but provides the least consistency:
            // the operation may yield very different results every time it is executed.
            private static void ExecuteReadUncommitted(TransactionalMemoryQueue queue, ICollection<QueueItem> items)
            {
                using (var enumerator = new QueueItemEnumerator(queue))
                {
                    while (enumerator.MoveNext())
                    {
                        Select(enumerator.Current, items);
                    }
                }
            }

            private static void Select(QueueItem item, ICollection<QueueItem> items)
            {
                if (item.IsDeleted)
                {
                    return;
                }
                items.Add(item);
            }

            protected override void InDoubt(TransactionalMemoryQueue queue, IEnumerable<QueueItem> items, IsolationLevel isolationLevel) =>
                Complete(queue, items, isolationLevel);

            protected override void Commit(TransactionalMemoryQueue queue, IEnumerable<QueueItem> items, IsolationLevel isolationLevel) =>
                Complete(queue, items, isolationLevel);

            protected override void Rollback(TransactionalMemoryQueue queue, IEnumerable<QueueItem> items, IsolationLevel isolationLevel) =>
                Complete(queue, items, isolationLevel);

            // When a read-operation is completed, all it needs to do is release all the locks is still holds, since no modifications were made
            // to the items in the queue. This only applies to isolation-levels that actually cause locks to be held for the duration of the
            // whole transaction.
            private static void Complete(TransactionalMemoryQueue queue, IEnumerable<QueueItem> items, IsolationLevel isolationLevel)
            {
                switch (isolationLevel)
                {
                    case IsolationLevel.Serializable:
                        CompleteSerializable(items, queue);
                        return;
                    case IsolationLevel.RepeatableRead:
                        CompleteRepeatableRead(items);
                        return;
                }
            }

            private static void CompleteSerializable(IEnumerable<QueueItem> items, TransactionalMemoryQueue queue)
            {
                try
                {
                    CompleteRepeatableRead(items);
                }
                finally
                {
                    queue._itemsLock.ExitUpgradeableReadLock();
                }
            }

            private static void CompleteRepeatableRead(IEnumerable<QueueItem> items)
            {
                foreach (var item in items)
                {
                    item.ExitUpgradeableReadLock();
                }
            }
        }

        #endregion

        #region [====== EnqueueOperation ======]

        // The enqueue-operation represents an operation where a set of new messages are appended to the end
        // of the queue. 
        private sealed class EnqueueOperation : TransactionOperation
        {
            private readonly IReadOnlyCollection<MicroServiceBusMessage> _messages;

            public EnqueueOperation(IReadOnlyCollection<MicroServiceBusMessage> messages)
            {
                _messages = messages;
            }

            // When executing the enqueue-operation, the operation first acquires a write-lock on the queue, regardless of the
            // isolation-level. As soon as it is acquired, the new items can be added to the end of the queue. As they are added,
            // the operation first acquires write-locks on the new items and then immediately releases the write-lock on the queue
            // so that other transactions can proceed with their read- and write-operations.
            protected override void Execute(TransactionalMemoryQueue queue, ICollection<QueueItem> items, IsolationLevel isolationLevel)
            {
                if (queue._itemsLock.TryEnterWriteLock(queue._lockTimeout))
                {
                    try
                    {
                        EnqueueItems(queue, items);
                        return;
                    }
                    finally
                    {
                        queue._itemsLock.ExitWriteLock();
                    }
                }
                throw NewLockTimeoutException(queue._lockTimeout);
            }

            private void EnqueueItems(TransactionalMemoryQueue queue, ICollection<QueueItem> items)
            {
                foreach (var message in _messages)
                {
                    queue._items.AddLast(CreateItem(queue, items, message));
                }
            }

            private static QueueItem CreateItem(TransactionalMemoryQueue queue, ICollection<QueueItem> items, MicroServiceBusMessage message)
            {
                var item = new QueueItem(message);
                if (item.EnterWriteLock(queue._lockTimeout))
                {
                    items.Add(item);
                    return item;
                }
                throw NewLockTimeoutException(queue._lockTimeout);
            }

            // When a transaction in which inserted some new messages remains in doubt (e.g. because the connection to the database was dropped),
            // we treat it as if it failed and needs to rollback the changes. The reason we do this is because it is generally better not to store
            // and send any messages when they actually should have been sent, than to send any messages that should never have been sent.
            protected override void InDoubt(TransactionalMemoryQueue queue, IEnumerable<QueueItem> items, IsolationLevel isolationLevel) =>
                Rollback(queue, items, isolationLevel);

            protected override void Commit(TransactionalMemoryQueue queue, IEnumerable<QueueItem> items, IsolationLevel isolationLevel) =>
                Complete(items, false);

            protected override void Rollback(TransactionalMemoryQueue queue, IEnumerable<QueueItem> items, IsolationLevel isolationLevel) =>
                Complete(items, true);

            // When committing or rolling back an enqueue-operation, the IsDeleted-flag of every added item is set to the appropriate value,
            // depending on the outcome of the transaction. This means that when rolling back the operation, the items remain in the queue, but will
            // be marked as being deleted items that will be cleanup later by the garbage collector in this queue. The reason this is implemented
            // like this is that we want to prevent having to obtain a write-lock on the queue, possibly leading to wait-times or exceptions,
            // while were in the sensitive phase of completing a transaction.
            private static void Complete(IEnumerable<QueueItem> items, bool isDeleted)
            {
                foreach (var item in items)
                {
                    item.IsDeleted = isDeleted;
                    item.IsCommitted = true;
                    item.ExitWriteLock();
                }
            }
        }

        #endregion

        #region [====== DequeueOperation ======]

        // The dequeue-operation represents an operation where a set of messages are removed from the front of the queue.
        // On a statement-level, it's like a combination of a read-operation (scanning the first elements of the queue)
        // and a write-operation (removing them as write-locks are acquired).
        private sealed class DequeueOperation : TransactionOperation
        {
            private readonly int _batchSize;

            public DequeueOperation(BatchSize batchSize)
            {
                _batchSize = batchSize;
            }

            protected override void Execute(TransactionalMemoryQueue queue, ICollection<QueueItem> items, IsolationLevel isolationLevel)
            {
                switch (isolationLevel)
                {
                    case IsolationLevel.Serializable:
                        ExecuteSerializable(queue, items);
                        return;
                    case IsolationLevel.RepeatableRead:
                    case IsolationLevel.ReadCommitted:
                    case IsolationLevel.ReadUncommitted:
                        Execute(queue, items);
                        return;
                    default:
                        throw NewIsolationLevelNotSupportedException(isolationLevel);
                }
            }

            // When the isolation-level is Serializable, we first obtain a read-lock on the queue, just like the
            // select-operation, to prevent other transactions to mutate the queue while this transaction in still
            // in progress.
            private void ExecuteSerializable(TransactionalMemoryQueue queue, ICollection<QueueItem> items)
            {
                if (queue._itemsLock.TryEnterReadLock(queue._lockTimeout))
                {
                    Execute(queue, items);
                    return;
                }
                throw NewLockTimeoutException(queue._lockTimeout);
            }

            // Regardless of the isolation-level, the operation iterates over the queue until it has acquired write-locks
            // on all items that need to be dequeued. Note that dequeue-ing an item in this stage only means it is marked as
            // deleted and returned in the result-set. Once committed, the queue-garbage-collector will eventually remove these
            // items from the queue.
            private void Execute(TransactionalMemoryQueue queue, ICollection<QueueItem> items)
            {
                using (var enumerator = new QueueItemEnumerator(queue))
                {
                    var itemsToDequeue = _batchSize;

                    while (itemsToDequeue > 0 && enumerator.MoveNext())
                    {
                        if (TryDequeue(enumerator.Current, queue._lockTimeout))
                        {
                            items.Add(enumerator.Current);
                            itemsToDequeue--;
                        }
                    }
                }
            }

            // An item is dequeued if the write-lock is acquired and the item hasn't been deleted (dequeued) yet.
            private static bool TryDequeue(QueueItem item, TimeSpan timeout)
            {
                if (item.EnterWriteLock(timeout))
                {
                    if (item.IsDeleted)
                    {
                        item.ExitWriteLock();
                        return false;
                    }
                    item.IsDeleted = true;
                    return true;
                }
                return false;
            }

            // Similar to the enqueue-operation, the situation of a transaction that is in doubt is treated as a failure and causes the
            // the changes to be rolled back. In this case that means that the messages that were dequeued during the transaction might have
            // been processed/send successfully but remain in the queue, possibly causing them to be processed more than once. This is, however,
            // less of a problem than if the messages were to be lost entirely.
            protected override void InDoubt(TransactionalMemoryQueue queue, IEnumerable<QueueItem> items, IsolationLevel isolationLevel) =>
                Rollback(queue, items, isolationLevel);

            protected override void Commit(TransactionalMemoryQueue queue, IEnumerable<QueueItem> items, IsolationLevel isolationLevel) =>
                Complete(queue, items, isolationLevel, true);

            protected override void Rollback(TransactionalMemoryQueue queue, IEnumerable<QueueItem> items, IsolationLevel isolationLevel) =>
                Complete(items, false);

            // When committing or rolling back an dequeue-operation, the IsDeleted-flag of every removed item is set to the appropriate value,
            // depending on the outcome of the transaction.
            private static void Complete(TransactionalMemoryQueue queue, IEnumerable<QueueItem> items, IsolationLevel isolationLevel, bool isDeleted)
            {
                switch (isolationLevel)
                {
                    case IsolationLevel.Serializable:
                        CompleteSerializable(items, isDeleted, queue);
                        return;
                    case IsolationLevel.RepeatableRead:
                    case IsolationLevel.ReadCommitted:
                    case IsolationLevel.ReadUncommitted:
                        Complete(items, isDeleted);
                        return;
                }
            }

            private static void CompleteSerializable(IEnumerable<QueueItem> items, bool isDeleted, TransactionalMemoryQueue queue)
            {
                try
                {
                    Complete(items, isDeleted);
                }
                finally
                {
                    queue._itemsLock.ExitReadLock();
                }
            }

            private static void Complete(IEnumerable<QueueItem> items, bool isDeleted)
            {
                foreach (var item in items)
                {
                    item.IsDeleted = isDeleted;
                    item.IsCommitted = true;
                    item.ExitWriteLock();
                }
            }
        }

        #endregion

        #region [====== QueueItem ======]

        // A QueueItem is similar to a row in a table of a database: it contains
        // the relevant data (the message) and allows read- and write-locks to be
        // acquired on it, depending on the intent of the transaction and transaction
        // isolation level.
        private sealed class QueueItem : AsyncDisposable
        {
            private readonly ReaderWriterLockSlim _itemLock;
            private readonly MicroServiceBusMessage _message;
            private int _isDeleted;
            private int _isCommitted;

            public QueueItem(MicroServiceBusMessage message)
            {
                _itemLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
                _message = message;
            }

            public override string ToString()
            {
                var item = new StringBuilder(_message.ToString()).Append(IsCommitted ? " | COMMITTED" : " | UNCOMMITTED");

                if (IsDeleted)
                {
                    item = item.Append(" | DELETED");
                }
                return item.ToString();
            }

            public bool IsDeletedPermanently =>
                IsDeleted && IsCommitted;

            public bool IsDeleted
            {
                get => GetValue(_isDeleted);
                set => SetValue(ref _isDeleted, value);
            }

            public bool IsCommitted
            {
                get => GetValue(_isCommitted);
                set => SetValue(ref _isCommitted, value);
            }

            private static bool GetValue(int value) =>
                value == 1;

            private static void SetValue(ref int oldValue, bool newValue) =>
                Interlocked.Exchange(ref oldValue, newValue ? 1 : 0);

            public MicroServiceBusMessage Message =>
                _message;

            public bool EnterReadLock(TimeSpan timeout) =>
                AcquireLock(_itemLock.TryEnterReadLock, timeout, _itemLock.ExitReadLock);

            public void ExitReadLock() =>
                _itemLock.ExitReadLock();

            public bool EnterUpgradeableReadLock(TimeSpan timeout) =>
                AcquireLock(_itemLock.TryEnterUpgradeableReadLock, timeout, _itemLock.ExitUpgradeableReadLock);

            public void ExitUpgradeableReadLock() =>
                _itemLock.ExitUpgradeableReadLock();

            public bool EnterWriteLock(TimeSpan timeout) =>
                AcquireLock(_itemLock.TryEnterWriteLock, timeout, _itemLock.ExitWriteLock);

            public void ExitWriteLock() =>
                _itemLock.ExitWriteLock();

            private bool AcquireLock(Func<TimeSpan, bool> enterLockMethod, TimeSpan timeout, Action exitLockMethod)
            {
                if (enterLockMethod.Invoke(timeout))
                {
                    if (IsDeletedPermanently)
                    {
                        exitLockMethod.Invoke();
                        return false;
                    }
                    return true;
                }
                throw NewLockTimeoutException(timeout);
            }

            protected override ValueTask DisposeAsync(DisposeContext context)
            {
                if (context != DisposeContext.Finalizer)
                {
                    _itemLock.Dispose();
                }
                return base.DisposeAsync(context);
            }
        }

        #endregion

        #region [====== QueueItemEnumerator ======]

        // This enumerator serves to enumerate the items in the queue from first to last
        // in a thread-safe manner. It does so by obtaining and releasing a read-lock on
        // the queue per item. The effect is that the queue might change while the iteration
        // is in progress, but given the nature of handling concurrent transactions, this is
        // actually a desired effect.
        private sealed class QueueItemEnumerator : Disposable, IEnumerator<QueueItem>
        {
            private readonly TransactionalMemoryQueue _queue;
            private LinkedListNode<QueueItem> _current;

            public QueueItemEnumerator(TransactionalMemoryQueue queue)
            {
                _queue = queue;
            }

            object IEnumerator.Current =>
                Current;

            public QueueItem Current =>
                _current.Value;

            public bool MoveNext()
            {
                if (_queue._itemsLock.TryEnterReadLock(_queue._lockTimeout))
                {
                    try
                    {
                        if (TryMoveNext(out var next))
                        {
                            _current = next;
                            return true;
                        }
                        return false;
                    }
                    finally
                    {
                        _queue._itemsLock.ExitReadLock();
                    }
                }
                throw NewLockTimeoutException(_queue._lockTimeout);
            }

            // The enumerator skips over items that represent deleted items, since these
            // items are scheduled to be removed from the queue entirely and should not be
            // accessible by any transaction.
            private bool TryMoveNext(out LinkedListNode<QueueItem> next)
            {
                next = _current;

                do
                {
                    if ((next = MoveNext(next)) == null)
                    {
                        return false;
                    }
                } while (next.Value.IsDeletedPermanently);

                return true;
            }

            private LinkedListNode<QueueItem> MoveNext(LinkedListNode<QueueItem> current) =>
                current == null ? _queue._items.First : current.Next;

            public void Reset() =>
                _current = null;
        }

        #endregion

        private readonly IMessageSerializer _serializer;
        //private readonly Timer _garbageCollectorTimer;
        private readonly Dictionary<string, TransactionOperationManager> _transactionOperations;
        private readonly LinkedList<QueueItem> _items;
        private readonly ReaderWriterLockSlim _itemsLock;
        private readonly TimeSpan _lockTimeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionalMemoryQueue" /> class.
        /// </summary>
        /// <param name="serializer">The serializer that is used to serialize and deserialize all messages that are stored in this queue.</param>
        /// <param name="lockTimeout">The maximum amount of time a transaction must wait for a lock te become available.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="lockTimeout"/> is negative.
        /// </exception>
        public TransactionalMemoryQueue(IMessageSerializer serializer = null, TimeSpan? lockTimeout = null)
        {
            _serializer = serializer ?? new MessageSerializer();
            _transactionOperations = new Dictionary<string, TransactionOperationManager>();
            _items = new LinkedList<QueueItem>();
            _itemsLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _lockTimeout = ToTimeSpan(lockTimeout);
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().FriendlyName()} (About {_items.Count} item(s) in queue, {_transactionOperations.Count} transaction(s) in progress)";

        /// <inheritdoc />
        protected override IMessageSerializer Serializer =>
            _serializer;

        private static TimeSpan ToTimeSpan(TimeSpan? lockTimeout)
        {
            if (lockTimeout.HasValue)
            {
                if (lockTimeout.Value < TimeSpan.Zero)
                {
                    throw NewNegativeTimeoutNotAllowedException(lockTimeout.Value);
                }
                return lockTimeout.Value;
            }
            return TimeSpan.FromSeconds(30);
        }

        private static Exception NewNegativeTimeoutNotAllowedException(TimeSpan lockTimeout)
        {
            var messageFormat = ExceptionMessages.TransactionalMemoryQueue_NegativeTimeoutNotAllowed;
            var message = string.Format(messageFormat, lockTimeout);
            return new ArgumentOutOfRangeException(nameof(lockTimeout), message);
        }

        private static Exception NewLockTimeoutException(TimeSpan lockTimeout)
        {
            var messageFormat = ExceptionMessages.TransactionalMemoryQueue_LockTimeout;
            var message = string.Format(messageFormat, lockTimeout);
            return new TimeoutException(message);
        }

        #region [====== CountAsync ======]

        /// <inheritdoc />
        protected override async Task<int> CountItemsAsync()
        {
            using (var scope = NewTransactionScope())
            {
                var messages = await CountItemsAsync(Transaction.Current);
                scope.Complete();
                return messages.Count;
            }
        }

        private Task<IReadOnlyCollection<MicroServiceBusMessage>> CountItemsAsync(Transaction transaction) =>
            ExecuteAsync(new SelectOperation(), transaction);

        #endregion

        #region [====== EnqueueAsync ======]

        /// <inheritdoc />
        protected override async Task EnqueueAsync(IReadOnlyCollection<MicroServiceBusMessage> messages)
        {
            using (var scope = NewTransactionScope())
            {
                await EnqueueAsync(messages, Transaction.Current);
                scope.Complete();
            }
        }

        private Task EnqueueAsync(IReadOnlyCollection<MicroServiceBusMessage> messages, Transaction transaction) =>
            ExecuteAsync(new EnqueueOperation(messages), transaction);

        #endregion

        #region [====== DequeueAsync ======]

        /// <inheritdoc />
        protected override async Task<IEnumerable<MicroServiceBusMessage>> DequeueAsync(BatchSize batchSize)
        {
            using (var scope = NewTransactionScope())
            {
                var messages = await DequeueAsync(batchSize, Transaction.Current);
                scope.Complete();
                return messages;
            }
        }

        private Task<IReadOnlyCollection<MicroServiceBusMessage>> DequeueAsync(BatchSize batchSize, Transaction transaction) =>
            ExecuteAsync(new DequeueOperation(batchSize), transaction);

        #endregion

        #region [====== ExecuteAsync ======]

        private Task<IReadOnlyCollection<MicroServiceBusMessage>> ExecuteAsync(TransactionOperation operation, Transaction transaction) =>
            GetOrAddOperationManager(transaction).ExecuteAsync(operation);

        private TransactionOperationManager GetOrAddOperationManager(Transaction transaction)
        {
            if (transaction.TransactionInformation.Status == TransactionStatus.Active)
            {
                lock (_transactionOperations)
                {
                    var transactionId = transaction.TransactionInformation.LocalIdentifier;

                    if (_transactionOperations.TryGetValue(transactionId, out var manager))
                    {
                        return manager;
                    }
                    _transactionOperations[transactionId] = manager = new TransactionOperationManager(this, transaction);

                    try
                    {
                        return manager.Start();
                    }
                    catch
                    {
                        manager.Dispose();
                        throw;
                    }
                    finally
                    {
                        RemoveOperationManagerOnCompleted(transaction, transactionId);
                    }
                }
            }
            throw NewTransactionAlreadyCompletedException(transaction);
        }

        private void RemoveOperationManagerOnCompleted(Transaction transaction, string transactionId)
        {
            transaction.TransactionCompleted += (s, e) =>
            {
                lock (_transactionOperations)
                {
                    if (_transactionOperations.TryGetValue(transactionId, out var manager))
                    {
                        _transactionOperations.Remove(transactionId);

                        manager.Dispose();
                    }
                }
            };
        }

        private static TransactionScope NewTransactionScope() =>
            new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

        private static Exception NewTransactionAlreadyCompletedException(Transaction transaction)
        {
            var messageFormat = ExceptionMessages.TransactionalMemoryQueue_TransactionAlreadyCompleted;
            var message = string.Format(messageFormat, transaction.TransactionInformation.LocalIdentifier, transaction.TransactionInformation.Status);
            return new InvalidOperationException(message);
        }

        private static Exception NewIsolationLevelNotSupportedException(IsolationLevel isolationLevel) =>
            NewIsolationLevelNotSupportedException(isolationLevel, IsolationLevel.Serializable, IsolationLevel.RepeatableRead, IsolationLevel.ReadCommitted, IsolationLevel.ReadUncommitted);

        private static Exception NewIsolationLevelNotSupportedException(IsolationLevel isolationLevel, params IsolationLevel[] supportedLevels)
        {
            var messageFormat = ExceptionMessages.TransactionalMemoryQueue_IsolationLevelNotSupported;
            var message = string.Format(messageFormat, isolationLevel, string.Join(" | ", supportedLevels));
            return new InvalidOperationException(message);
        }

        #endregion

        #region [====== DisposeAsync ======]

        /// <inheritdoc />
        protected override async ValueTask DisposeAsync(DisposeContext context)
        {
            if (context != DisposeContext.Finalizer)
            {
                _itemsLock.Dispose();
            }
            await base.DisposeAsync(context);
        }

        #endregion
    }
}
