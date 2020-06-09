using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Kingo.MicroServices.MessageFactoryTest;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class TransactionalMemoryQueueTest
    {
        #region [====== EnqueueAsync, DequeueAsync & CountAsync (Implicit Transactions) ======]

        [TestMethod]
        public async Task CountAsync_ReturnsZero_IfQueueIsStillEmpty()
        {
            await using (var queue = CreateMemoryQueue())
            {
                Assert.AreEqual(0, await queue.CountAsync());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task CountAsync_Throws_IfQueueHasBeenDisposed()
        {
            await using (var queue = CreateMemoryQueue())
            {
                await queue.DisposeAsync();
                await queue.CountAsync();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EnqueueAsync_Throws_IfMessagesIsNull()
        {
            await using (var queue = CreateMemoryQueue())
            {
                await queue.EnqueueAsync(null);
            }
        }

        [TestMethod]
        public async Task EnqueueAsync_EnqueuesNoMessages_IfMessageCollectionIsEmpty()
        {
            await using (var queue = CreateMemoryQueue())
            {
                await queue.EnqueueAsync(Enumerable.Empty<IMessage>());

                Assert.AreEqual(0, await queue.CountAsync());
            }
        }

        [TestMethod]
        public async Task EnqueueAsync_EnqueuesSpecifiedMessages_IfMessageCollectionIsNotEmpty_And_NoTransactionIsRunning()
        {
            await using (var queue = CreateMemoryQueue())
            {
                var messages = CreateOutputMessages();

                await queue.EnqueueAsync(messages);

                Assert.AreEqual(messages.Length, await queue.CountAsync());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task EnqueueAsync_Throws_IfQueueHasBeenDisposed()
        {
            await using (var queue = CreateMemoryQueue())
            {
                await queue.DisposeAsync();
                await queue.EnqueueAsync(Enumerable.Empty<IMessage>());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public async Task DequeueAsync_Throws_IfBatchSizeIsSmallerThanMinimumBatchSize()
        {
            await using (var queue = CreateMemoryQueue())
            {
                await queue.DequeueAsync(0);
            }
        }

        [TestMethod]
        public async Task DequeueAsync_ReturnsEmptyCollection_IfBatchSizeIsEqualToMinimumBatchSize_But_QueueIsEmpty()
        {
            await using (var queue = CreateMemoryQueue())
            {
                var messages = await queue.DequeueAsync(BatchSize.MinValue);

                Assert.AreEqual(0, messages.Count);
                Assert.AreEqual(0, await queue.CountAsync());
            }
        }

        [TestMethod]
        public async Task DequeueAsync_ReturnsCollectionOfExpectedSize_IfBatchSizeIsSmallerThanQueueSize()
        {
            const int batchSize = 5;

            await using (var queue = CreateMemoryQueue())
            {
                await queue.EnqueueAsync(CreateOutputMessages(batchSize + 1));

                var queueCount = await queue.CountAsync();
                var messages = await queue.DequeueAsync(batchSize);

                Assert.AreEqual(batchSize, messages.Count);
                Assert.AreEqual(queueCount - batchSize, await queue.CountAsync());
            }
        }

        [TestMethod]
        public async Task DequeueAsync_ReturnsCollectionOfExpectedSize_IfBatchSizeIsGreaterThanQueueSize()
        {
            await using (var queue = CreateMemoryQueue())
            {
                var messagesA = CreateOutputMessages(10);

                await queue.EnqueueAsync(messagesA);

                var messagesB = await queue.DequeueAsync(messagesA.Length + 1);

                Assert.AreEqual(messagesA.Length, messagesB.Count);
                Assert.AreEqual(0, await queue.CountAsync());
            }
        }

        [TestMethod]
        public async Task DequeueAsync_ReturnsCollectionOfExpectedSize_IfMultipleSetsAreDequeuedInSeparateTransactions()
        {
            const int batchSize = 5;

            await using (var queue = CreateMemoryQueue())
            {
                await queue.EnqueueAsync(CreateOutputMessages(batchSize * 3));

                var queueCount = await queue.CountAsync();
                var messagesA = await queue.DequeueAsync(batchSize);
                var messagesB = await queue.DequeueAsync(batchSize);

                Assert.AreEqual(batchSize, messagesA.Count);
                Assert.AreEqual(batchSize, messagesB.Count);
                Assert.AreEqual(queueCount - 2 * batchSize, await queue.CountAsync());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task DequeueAsync_Throws_IfQueueHasBeenDisposed()
        {
            await using (var queue = CreateMemoryQueue())
            {
                await queue.DisposeAsync();
                await queue.DequeueAsync();
            }
        }

        #endregion

        #region [====== EnqueueAsync, DequeueAsync & CountAsync (Explicit Transactions, Invalid IsolationLevels) ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task CountAsync_Throws_IfIsolationLevelIsSnapshot()
        {
            await using (var queue = CreateMemoryQueue())
            {
                using (var scope = CreateTransactionScope(IsolationLevel.Snapshot))
                {
                    await queue.CountAsync();
                    scope.Complete();
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task CountAsync_Throws_IfIsolationLevelIsChaos()
        {
            await using (var queue = CreateMemoryQueue())
            {
                using (var scope = CreateTransactionScope(IsolationLevel.Chaos))
                {
                    await queue.CountAsync();
                    scope.Complete();
                }
            }
        }

        #endregion

        #region [====== EnqueueAsync, DequeueAsync & CountAsync (Explicit Transactions, Serializable) ======]

        [TestMethod]
        public async Task EnqueueAsync_EnqueuesSpecifiedMessages_IfMultipleSetsAreAddedInTheSameTransaction_And_IsolationLevelIsSerializable()
        {
            await EnqueueAsync_EnqueuesSpecifiedMessages_IfMultipleSetsAreAddedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel.Serializable);
        }

        [TestMethod]
        public async Task EnqueueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIsSerializable()
        {
            await EnqueueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIs(IsolationLevel.Serializable);
        }

        [TestMethod]
        public async Task DequeueAsync_DequeuesExpectedMessages_IfMultipleSetsAreRemovedInTheSameTransaction_And_IsolationLevelIsSerializable()
        {
            await DequeueAsync_DequeuesExpectedMessages_IfMultipleSetsAreRemovedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel.Serializable);
        }

        [TestMethod]
        public async Task DequeueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIsSerializable()
        {
            await DequeueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIs(IsolationLevel.Serializable);
        }

        [TestMethod]
        public async Task DequeueAsync_DequeuesExpectedMessages_IfMessagesWereEnqueuedInTheSameTransaction_And_IsolationLevelIsSerializable()
        {
            await DequeueAsync_DequeuesExpectedMessages_IfMessagesWereEnqueuedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel.Serializable);
        }

        [TestMethod]
        public async Task DequeueAsync_RollsbackChanges_IfMessagesWereEnqueuedInTheSameTransaction_And_TransactionIsAborted_And_IsolationLevelIsSerializable()
        {
            await DequeueAsync_RollsbackChanges_IfMessagesWereEnqueuedInTheSameTransaction_And_TransactionIsAborted_And_IsolationLevelIs(IsolationLevel.Serializable);
        }

        [TestMethod]
        public async Task CountAsync_DoesNotCauseTimeout_IfExecutedWhileOtherTransactionIsNotHoldingAnyLocks_And_IsolationLevelIsSerializable()
        {
            await CountAsync_DoesNotCauseTimeout_IfExecutedWhileOtherTransactionIsNotHoldingAnyLocks_And_IsolationLevelIs(IsolationLevel.Serializable);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public async Task CountAsync_CausesTimeout_IfExecutedWhileOtherTransactionStillHoldsUpgradeableReadLocks_And_IsolationLevelIsSerializable()
        {
            await CountAsync_CausesTimeout_IfExecutedWhileOtherTransactionStillHoldsUpgradeableReadLocks_And_IsolationLevelIs(IsolationLevel.Serializable);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public async Task CountAsync_CausesTimeout_IfExecutedWhileOtherTransactionStillHoldsWriteLocks_And_IsolationLevelIsSerializable()
        {
            await CountAsync_CausesTimeout_IfExecutedWhileOtherTransactionStillHoldsWriteLocks_And_IsolationLevelIs(IsolationLevel.Serializable);
        }

        #endregion

        #region [====== EnqueueAsync, DequeueAsync & CountAsync (Explicit Transactions, RepeatableRead) ======]

        [TestMethod]
        public async Task EnqueueAsync_EnqueuesSpecifiedMessages_IfMultipleSetsAreAddedInTheSameTransaction_And_IsolationLevelIsRepeatableRead()
        {
            await EnqueueAsync_EnqueuesSpecifiedMessages_IfMultipleSetsAreAddedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel.RepeatableRead);
        }

        [TestMethod]
        public async Task EnqueueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIsRepeatableRead()
        {
            await EnqueueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIs(IsolationLevel.RepeatableRead);
        }

        [TestMethod]
        public async Task DequeueAsync_DequeuesExpectedMessages_IfMultipleSetsAreRemovedInTheSameTransaction_And_IsolationLevelIsRepeatableRead()
        {
            await DequeueAsync_DequeuesExpectedMessages_IfMultipleSetsAreRemovedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel.RepeatableRead);
        }

        [TestMethod]
        public async Task DequeueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIsRepeatableRead()
        {
            await DequeueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIs(IsolationLevel.RepeatableRead);
        }

        [TestMethod]
        public async Task DequeueAsync_DequeuesExpectedMessages_IfMessagesWereEnqueuedInTheSameTransaction_And_IsolationLevelIsRepeatableRead()
        {
            await DequeueAsync_DequeuesExpectedMessages_IfMessagesWereEnqueuedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel.RepeatableRead);
        }

        [TestMethod]
        public async Task DequeueAsync_RollsbackChanges_IfMessagesWereEnqueuedInTheSameTransaction_And_TransactionIsAborted_And_IsolationLevelIsRepeatableRead()
        {
            await DequeueAsync_RollsbackChanges_IfMessagesWereEnqueuedInTheSameTransaction_And_TransactionIsAborted_And_IsolationLevelIs(IsolationLevel.RepeatableRead);
        }

        [TestMethod]
        public async Task CountAsync_DoesNotCauseTimeout_IfExecutedWhileOtherTransactionIsNotHoldingAnyLocks_And_IsolationLevelIsRepeatableRead()
        {
            await CountAsync_DoesNotCauseTimeout_IfExecutedWhileOtherTransactionIsNotHoldingAnyLocks_And_IsolationLevelIs(IsolationLevel.RepeatableRead);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public async Task CountAsync_CausesTimeout_IfExecutedWhileOtherTransactionStillHoldsUpgradeableReadLocks_And_IsolationLevelIsRepeatableRead()
        {
            await CountAsync_CausesTimeout_IfExecutedWhileOtherTransactionStillHoldsUpgradeableReadLocks_And_IsolationLevelIs(IsolationLevel.RepeatableRead);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public async Task CountAsync_CausesTimeout_IfExecutedWhileOtherTransactionStillHoldsWriteLocks_And_IsolationLevelIsRepeatableRead()
        {
            await CountAsync_CausesTimeout_IfExecutedWhileOtherTransactionStillHoldsWriteLocks_And_IsolationLevelIs(IsolationLevel.RepeatableRead);
        }

        #endregion

        #region [====== EnqueueAsync, DequeueAsync & CountAsync (Explicit Transactions, ReadCommitted) ======]

        [TestMethod]
        public async Task EnqueueAsync_EnqueuesSpecifiedMessages_IfMultipleSetsAreAddedInTheSameTransaction_And_IsolationLevelIsReadCommitted()
        {
            await EnqueueAsync_EnqueuesSpecifiedMessages_IfMultipleSetsAreAddedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel.ReadCommitted);
        }

        [TestMethod]
        public async Task EnqueueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIsReadCommitted()
        {
            await EnqueueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIs(IsolationLevel.ReadCommitted);
        }

        [TestMethod]
        public async Task DequeueAsync_DequeuesExpectedMessages_IfMultipleSetsAreRemovedInTheSameTransaction_And_IsolationLevelIsReadCommitted()
        {
            await DequeueAsync_DequeuesExpectedMessages_IfMultipleSetsAreRemovedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel.ReadCommitted);
        }

        [TestMethod]
        public async Task DequeueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIsReadCommitted()
        {
            await DequeueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIs(IsolationLevel.ReadCommitted);
        }

        [TestMethod]
        public async Task DequeueAsync_DequeuesExpectedMessages_IfMessagesWereEnqueuedInTheSameTransaction_And_IsolationLevelIsReadCommitted()
        {
            await DequeueAsync_DequeuesExpectedMessages_IfMessagesWereEnqueuedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel.ReadCommitted);
        }

        [TestMethod]
        public async Task DequeueAsync_RollsbackChanges_IfMessagesWereEnqueuedInTheSameTransaction_And_TransactionIsAborted_And_IsolationLevelIsReadCommitted()
        {
            await DequeueAsync_RollsbackChanges_IfMessagesWereEnqueuedInTheSameTransaction_And_TransactionIsAborted_And_IsolationLevelIs(IsolationLevel.ReadCommitted);
        }

        [TestMethod]
        public async Task CountAsync_DoesNotCauseTimeout_IfExecutedWhileOtherTransactionIsNotHoldingAnyLocks_And_IsolationLevelIsReadCommitted()
        {
            await CountAsync_DoesNotCauseTimeout_IfExecutedWhileOtherTransactionIsNotHoldingAnyLocks_And_IsolationLevelIs(IsolationLevel.ReadCommitted);
        }

        [TestMethod]
        public async Task CountAsync_DoesNotCauseTimeout_IfExecutedWhileOtherTransactionStillHoldsUpgradeableReadLocks_And_IsolationLevelIsReadCommitted()
        {
            await CountAsync_DoesNotCauseTimeout_IfExecutedWhileOtherTransactionStillHoldsUpgradeableReadLocks_And_IsolationLevelIs(IsolationLevel.ReadCommitted);
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public async Task CountAsync_CausesTimeout_IfExecutedWhileOtherTransactionStillHoldsWriteLocks_And_IsolationLevelIsReadCommitted()
        {
            await CountAsync_CausesTimeout_IfExecutedWhileOtherTransactionStillHoldsWriteLocks_And_IsolationLevelIs(IsolationLevel.ReadCommitted);
        }

        #endregion

        #region [====== EnqueueAsync, DequeueAsync & CountAsync (Explicit Transactions, ReadUncommitted) ======]

        [TestMethod]
        public async Task EnqueueAsync_EnqueuesSpecifiedMessages_IfMultipleSetsAreAddedInTheSameTransaction_And_IsolationLevelIsReadUncommitted()
        {
            await EnqueueAsync_EnqueuesSpecifiedMessages_IfMultipleSetsAreAddedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel.ReadUncommitted);
        }

        [TestMethod]
        public async Task EnqueueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIsReadUncommitted()
        {
            await EnqueueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIs(IsolationLevel.ReadUncommitted);
        }

        [TestMethod]
        public async Task DequeueAsync_DequeuesExpectedMessages_IfMultipleSetsAreRemovedInTheSameTransaction_And_IsolationLevelIsReadUncommitted()
        {
            await DequeueAsync_DequeuesExpectedMessages_IfMultipleSetsAreRemovedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel.ReadUncommitted);
        }

        [TestMethod]
        public async Task DequeueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIsReadUncommitted()
        {
            await DequeueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIs(IsolationLevel.ReadUncommitted);
        }

        [TestMethod]
        public async Task DequeueAsync_DequeuesExpectedMessages_IfMessagesWereEnqueuedInTheSameTransaction_And_IsolationLevelIsReadUncommitted()
        {
            await DequeueAsync_DequeuesExpectedMessages_IfMessagesWereEnqueuedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel.ReadUncommitted);
        }

        [TestMethod]
        public async Task DequeueAsync_RollsbackChanges_IfMessagesWereEnqueuedInTheSameTransaction_And_TransactionIsAborted_And_IsolationLevelIsReadUncommitted()
        {
            await DequeueAsync_RollsbackChanges_IfMessagesWereEnqueuedInTheSameTransaction_And_TransactionIsAborted_And_IsolationLevelIs(IsolationLevel.ReadUncommitted);
        }

        [TestMethod]
        public async Task CountAsync_DoesNotCauseTimeout_IfExecutedWhileOtherTransactionStillHoldsUpgradeableReadLocks_And_IsolationLevelIsReadUncommitted()
        {
            await CountAsync_DoesNotCauseTimeout_IfExecutedWhileOtherTransactionStillHoldsUpgradeableReadLocks_And_IsolationLevelIs(IsolationLevel.ReadUncommitted);
        }

        [TestMethod]
        public async Task CountAsync_DoesNotCauseTimeout_IfExecutedWhileOtherTransactionIsNotHoldingAnyLocks_And_IsolationLevelIsReadUncommitted()
        {
            await CountAsync_DoesNotCauseTimeout_IfExecutedWhileOtherTransactionIsNotHoldingAnyLocks_And_IsolationLevelIs(IsolationLevel.ReadUncommitted);
        }

        [TestMethod]
        public async Task CountAsync_DoesNotCauseTimeout_IfExecutedWhileOtherTransactionStillHoldsWriteLocks_And_IsolationLevelIsReadUncommitted()
        {
            await using (var queue = CreateMemoryQueue())
            {
                using (var outerScope = CreateTransactionScope(IsolationLevel.ReadCommitted))
                {
                    var messages = CreateOutputMessages();

                    // The outer-transaction will add and then hold on to the write-locks until it completes.
                    await queue.EnqueueAsync(messages);

                    // This transaction will not attempt to obtain read locks because the IsolationLevel is ReadUncommitted.
                    using (var innerScope = CreateTransactionScope(IsolationLevel.ReadUncommitted, TransactionScopeOption.RequiresNew))
                    {
                        Assert.AreEqual(messages.Length, await queue.CountAsync());
                        innerScope.Complete();
                    }
                    outerScope.Complete();
                }
            }
        }

        #endregion

        #region [====== EnqueueAsync, DequeueAsync & CountAsync (Explicit Transactions) ======]

        private async Task EnqueueAsync_EnqueuesSpecifiedMessages_IfMultipleSetsAreAddedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel isolationLevel)
        {
            await using (var queue = CreateMemoryQueue())
            {
                var messages = CreateOutputMessages();

                using (var scope = CreateTransactionScope(isolationLevel))
                {
                    await queue.EnqueueAsync(messages);

                    Assert.AreEqual(messages.Length, await queue.CountAsync());

                    await queue.EnqueueAsync(messages);

                    Assert.AreEqual(messages.Length * 2, await queue.CountAsync());
                    scope.Complete();
                }
                Assert.AreEqual(messages.Length * 2, await queue.CountAsync());
            }
        }

        private async Task EnqueueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIs(IsolationLevel isolationLevel)
        {
            await using (var queue = CreateMemoryQueue())
            {
                using (CreateTransactionScope(isolationLevel))
                {
                    var messageCount = await queue.EnqueueAsync(CreateOutputMessages());

                    Assert.AreEqual(messageCount, await queue.CountAsync());
                }
                Assert.AreEqual(0, await queue.CountAsync());
            }
        }

        private async Task DequeueAsync_DequeuesExpectedMessages_IfMultipleSetsAreRemovedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel isolationLevel)
        {
            const int batchSize = 5;

            await using (var queue = CreateMemoryQueue())
            {
                var messageCount = await queue.EnqueueAsync(CreateOutputMessages(batchSize * 3));

                using (var scope = CreateTransactionScope(isolationLevel))
                {
                    Assert.AreEqual(messageCount, await queue.CountAsync());

                    var messagesA = await queue.DequeueAsync(batchSize);

                    Assert.AreEqual(batchSize, messagesA.Count);
                    Assert.AreEqual(messageCount - batchSize, await queue.CountAsync());

                    var messagesB = await queue.DequeueAsync(batchSize);

                    Assert.AreEqual(batchSize, messagesB.Count);
                    Assert.AreEqual(messageCount - 2 * batchSize, await queue.CountAsync());
                    scope.Complete();
                }
                Assert.AreEqual(messageCount - 2 * batchSize, await queue.CountAsync());
            }
        }

        private async Task DequeueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIs(IsolationLevel isolationLevel)
        {
            const int batchSize = 5;

            await using (var queue = CreateMemoryQueue())
            {
                var messageCount = await queue.EnqueueAsync(CreateOutputMessages(batchSize * 3));

                using (CreateTransactionScope(isolationLevel))
                {
                    Assert.AreEqual(messageCount, await queue.CountAsync());

                    var messagesA = await queue.DequeueAsync(batchSize);

                    Assert.AreEqual(batchSize, messagesA.Count);
                    Assert.AreEqual(messageCount - batchSize, await queue.CountAsync());
                }
                Assert.AreEqual(messageCount, await queue.CountAsync());
            }
        }

        private async Task DequeueAsync_DequeuesExpectedMessages_IfMessagesWereEnqueuedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel isolationLevel)
        {
            const int batchSize = 5;

            await using (var queue = CreateMemoryQueue())
            {
                var messages = CreateOutputMessages(batchSize * 3);

                using (var scope = CreateTransactionScope(isolationLevel))
                {
                    await queue.EnqueueAsync(messages);

                    Assert.AreEqual(messages.Length, await queue.CountAsync());

                    var messagesA = await queue.DequeueAsync(batchSize);

                    Assert.AreEqual(batchSize, messagesA.Count);
                    Assert.AreEqual(messages.Length - batchSize, await queue.CountAsync());

                    var messagesB = await queue.DequeueAsync(batchSize);

                    Assert.AreEqual(batchSize, messagesB.Count);
                    Assert.AreEqual(messages.Length - 2 * batchSize, await queue.CountAsync());
                    scope.Complete();
                }
                Assert.AreEqual(messages.Length - 2 * batchSize, await queue.CountAsync());
            }
        }

        private async Task DequeueAsync_RollsbackChanges_IfMessagesWereEnqueuedInTheSameTransaction_And_TransactionIsAborted_And_IsolationLevelIs(IsolationLevel isolationLevel)
        {
            const int batchSize = 5;

            await using (var queue = CreateMemoryQueue())
            {
                using (CreateTransactionScope(isolationLevel))
                {
                    var messageCount = await queue.EnqueueAsync(CreateOutputMessages(batchSize * 3));

                    Assert.AreEqual(messageCount, await queue.CountAsync());

                    var messagesA = await queue.DequeueAsync(batchSize);

                    Assert.AreEqual(batchSize, messagesA.Count);
                    Assert.AreEqual(messageCount - batchSize, await queue.CountAsync());

                    var messagesB = await queue.DequeueAsync(batchSize);

                    Assert.AreEqual(batchSize, messagesB.Count);
                    Assert.AreEqual(messageCount - 2 * batchSize, await queue.CountAsync());
                }
                Assert.AreEqual(0, await queue.CountAsync());
            }
        }

        private async Task CountAsync_DoesNotCauseTimeout_IfExecutedWhileOtherTransactionIsNotHoldingAnyLocks_And_IsolationLevelIs(IsolationLevel isolationLevel)
        {
            await using (var queue = CreateMemoryQueue())
            {
                var messageCount = await queue.EnqueueAsync(CreateOutputMessages());

                using (var outerScope = CreateTransactionScope(IsolationLevel.ReadCommitted))
                {
                    // The outer-transaction will not hold on to any locks since it will release every
                    // read-lock as it iterates over the queue.
                    Assert.AreEqual(messageCount, await queue.CountAsync());

                    // Regardless of the locks acquired by the inner-transaction, it won't be blocked.
                    using (var innerScope = CreateTransactionScope(isolationLevel, TransactionScopeOption.RequiresNew))
                    {
                        Assert.AreEqual(messageCount, await queue.CountAsync());
                        innerScope.Complete();
                    }
                    outerScope.Complete();
                }
            }
        }

        private async Task CountAsync_DoesNotCauseTimeout_IfExecutedWhileOtherTransactionStillHoldsUpgradeableReadLocks_And_IsolationLevelIs(IsolationLevel isolationLevel)
        {
            await using (var queue = CreateMemoryQueue())
            {
                var messageCount = await queue.EnqueueAsync(CreateOutputMessages());

                using (var outerScope = CreateTransactionScope(IsolationLevel.RepeatableRead))
                {
                    // The outer-transaction will hold on to (upgradeable) read-locks until it completes.
                    Assert.AreEqual(messageCount, await queue.CountAsync());

                    // This transaction will only attempt to obtain simple read locks on those items (as long as IsolationLevel is RU or RC),
                    // which won't block the count-operation.
                    using (var innerScope = CreateTransactionScope(isolationLevel, TransactionScopeOption.RequiresNew))
                    {
                        Assert.AreEqual(messageCount, await queue.CountAsync());
                        innerScope.Complete();
                    }
                    outerScope.Complete();
                }
            }
        }

        private async Task CountAsync_CausesTimeout_IfExecutedWhileOtherTransactionStillHoldsUpgradeableReadLocks_And_IsolationLevelIs(IsolationLevel isolationLevel)
        {
            await using (var queue = CreateMemoryQueue())
            {
                var messageCount = await queue.EnqueueAsync(CreateOutputMessages());

                using (CreateTransactionScope(IsolationLevel.RepeatableRead))
                {
                    // The outer-transaction will hold on to (upgradeable) read-locks until it completes.
                    Assert.AreEqual(messageCount, await queue.CountAsync());

                    // This transaction will also attempt to obtain upgradeable read locks on those items (as long as IsolationLevel is RR or SZ),
                    // causing it to block until the locks are released. However, the outer transaction will not be completed, because we
                    // are awaiting the result of CountAsync here. As such, the timeout on acquiring the read-locks will expire and an exception
                    // will be thrown as a result.
                    using (CreateTransactionScope(isolationLevel, TransactionScopeOption.RequiresNew))
                    {
                        try
                        {
                            await queue.CountAsync();

                            throw NewTimeoutExpectedException();
                        }
                        catch (TimeoutException exception)
                        {
                            AssertIsLockTimeoutException(exception);
                            throw;
                        }
                    }
                }
            }
        }

        private async Task CountAsync_CausesTimeout_IfExecutedWhileOtherTransactionStillHoldsWriteLocks_And_IsolationLevelIs(IsolationLevel isolationLevel)
        {
            await using (var queue = CreateMemoryQueue())
            {
                using (CreateTransactionScope(IsolationLevel.ReadCommitted))
                {
                    // The outer-transaction will add and then hold on to the write-locks until it completes.
                    await queue.EnqueueAsync(CreateOutputMessages());

                    // This transaction will attempt to obtain read locks on those items (as long as IsolationLevel is not ReadUncommitted),
                    // causing it to block until the locks are released. However, the outer transaction will not be completed, because we
                    // are awaiting the result of CountAsync here. As such, the timeout on acquiring the read-locks will expire and an exception
                    // will be thrown as a result.
                    using (CreateTransactionScope(isolationLevel, TransactionScopeOption.RequiresNew))
                    {
                        try
                        {
                            await queue.CountAsync();

                            throw NewTimeoutExpectedException();
                        }
                        catch (TimeoutException exception)
                        {
                            AssertIsLockTimeoutException(exception);
                            throw;
                        }
                    }
                }
            }
        }

        private static void AssertIsLockTimeoutException(Exception exception) =>
            Assert.IsTrue(exception.Message.StartsWith("Transaction failed because a lock timeout expired"));

        private static Exception NewTimeoutExpectedException() =>
            new AssertFailedException("Expected timeout did not occur.");

        #endregion

        #region [====== Changed (Event) ======]

        [TestMethod]
        public async Task Changed_IsNotRaised_IfOnlyCountAsyncWasExecuted()
        {
            TransactionalQueueChangedEventArgs changedEventArgs = null;

            await using (var queue = CreateMemoryQueue())
            {
                queue.Changed += (s, e) =>
                {
                    changedEventArgs = e;
                };

                Assert.AreEqual(0, await queue.CountAsync());
            }
            AssertIsNull(changedEventArgs);
        }

        [TestMethod]
        public async Task Changed_IsNotRaised_IfEnqueueAsyncWasExecuted_But_MessagesCountWasZero()
        {
            TransactionalQueueChangedEventArgs changedEventArgs = null;

            await using (var queue = CreateMemoryQueue())
            {
                queue.Changed += (s, e) =>
                {
                    changedEventArgs = e;
                };

                Assert.AreEqual(0, await queue.EnqueueAsync(Enumerable.Empty<IMessage>()));
            }
            AssertIsNull(changedEventArgs);
        }

        [TestMethod]
        public async Task Changed_IsNotRaised_IfEnqueueAsyncWasExecuted_But_TransactionWasRolledBack()
        {
            TransactionalQueueChangedEventArgs changedEventArgs = null;

            await using (var queue = CreateMemoryQueue())
            {
                queue.Changed += (s, e) =>
                {
                    changedEventArgs = e;
                };

                using (CreateTransactionScope(IsolationLevel.ReadCommitted))
                {
                    await queue.EnqueueAsync(CreateOutputMessages());
                }
            }
            AssertIsNull(changedEventArgs);
        }

        [TestMethod]
        public async Task Changed_IsRaised_IfEnqueueAsyncWasExecuted_And_TransactionWasCommitted()
        {
            TransactionalQueueChangedEventArgs changedEventArgs = null;
            var messages = CreateOutputMessages();

            await using (var queue = CreateMemoryQueue())
            {
                queue.Changed += (s, e) =>
                {
                    changedEventArgs = e;
                };

                using (var scope = CreateTransactionScope(IsolationLevel.ReadCommitted))
                {
                    await queue.EnqueueAsync(messages);
                    scope.Complete();
                }
            }
            AssertChanges(changedEventArgs, messages.Length, 0);
        }

        [TestMethod]
        public async Task Changed_IsNotRaised_IfDequeueAsyncWasExecuted_But_MessagesCountWasZero()
        {
            TransactionalQueueChangedEventArgs changedEventArgs = null;

            await using (var queue = CreateMemoryQueue())
            {
                queue.Changed += (s, e) =>
                {
                    changedEventArgs = e;
                };

                var messages = await queue.DequeueAsync();

                Assert.AreEqual(0, messages.Count);
            }
            AssertIsNull(changedEventArgs);
        }

        [TestMethod]
        public async Task Changed_IsNotRaised_IfDequeueAsyncWasExecuted_But_TransactionWasRolledBack()
        {
            TransactionalQueueChangedEventArgs changedEventArgs = null;
            var messages = CreateOutputMessages();

            await using (var queue = CreateMemoryQueue())
            {
                Assert.AreEqual(messages.Length, await queue.EnqueueAsync(messages));

                queue.Changed += (s, e) =>
                {
                    changedEventArgs = e;
                };

                using (CreateTransactionScope(IsolationLevel.ReadCommitted))
                {
                    await queue.DequeueAsync(messages.Length);
                }
            }
            AssertIsNull(changedEventArgs);
        }

        [TestMethod]
        public async Task Changed_IsRaised_IfDequeueAsyncWasExecuted_And_TransactionWasCommitted()
        {
            TransactionalQueueChangedEventArgs changedEventArgs = null;
            var messages = CreateOutputMessages();

            await using (var queue = CreateMemoryQueue())
            {
                Assert.AreEqual(messages.Length, await queue.EnqueueAsync(messages));

                queue.Changed += (s, e) =>
                {
                    changedEventArgs = e;
                };

                using (var scope = CreateTransactionScope(IsolationLevel.ReadCommitted))
                {
                    await queue.DequeueAsync(messages.Length);
                    scope.Complete();
                }
            }
            AssertChanges(changedEventArgs, 0, messages.Length);
        }

        [TestMethod]
        public async Task Changed_IsRaised_IfBothEnqueueAsyncAndDequeueAsyncWereExecuted_And_TransactionWasCommitted()
        {
            const int batchSize = 5;
            TransactionalQueueChangedEventArgs changedEventArgs = null;
            var messages = CreateOutputMessages(2 * batchSize);

            await using (var queue = CreateMemoryQueue())
            {
                queue.Changed += (s, e) =>
                {
                    changedEventArgs = e;
                };

                using (var scope = CreateTransactionScope(IsolationLevel.ReadCommitted))
                {
                    await queue.EnqueueAsync(messages);
                    await queue.DequeueAsync(batchSize);
                    await queue.EnqueueAsync(messages);
                    await queue.DequeueAsync(batchSize);
                    await queue.DequeueAsync(batchSize);
                    scope.Complete();
                }
            }
            AssertChanges(changedEventArgs, 2 * messages.Length, 3 * batchSize);
        }

        private static void AssertIsNull(TransactionalQueueChangedEventArgs changedEventArgs) =>
            Assert.IsNull(changedEventArgs, "The changed-event should not have been raised.");

        private static void AssertChanges(TransactionalQueueChangedEventArgs changedEventArgs, int enqueueCount, int dequeueCount)
        {
            Assert.IsNotNull(changedEventArgs);
            Assert.AreEqual(enqueueCount, changedEventArgs.EnqueueCount);
            Assert.AreEqual(dequeueCount, changedEventArgs.DequeueCount);
        }

        #endregion

        private static TransactionScope CreateTransactionScope(IsolationLevel isolationLevel, TransactionScopeOption option = TransactionScopeOption.Required)
        {
            return new TransactionScope(option, new TransactionOptions()
            {
                IsolationLevel = isolationLevel
            }, TransactionScopeAsyncFlowOption.Enabled);
        }

        private static TransactionalMemoryQueue CreateMemoryQueue() =>
            new TransactionalMemoryQueue(new MessageSerializer(), TimeSpan.FromSeconds(1));

        private static IMessage[] CreateOutputMessages(int minimum = 1) =>
            CreateMessages(DateTimeOffset.UtcNow.Millisecond + minimum).ToArray();
    }
}
