using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Kingo.MicroServices.Configuration;
using Kingo.MicroServices.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Kingo.MicroServices.MessageFactoryTest;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class TransactionalMemoryQueueTest
    {
        #region [====== CountAsync, EnqueueAsync & DequeueAsync (Implicit Transactions) ======]

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

        #region [====== EnqueueAsync & DequeueAsync (Explicit Transactions, Invalid IsolationLevels) ======]

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

        #region [====== EnqueueAsync & DequeueAsync (Explicit Transactions, Serializable) ======]

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

        #endregion

        #region [====== EnqueueAsync & DequeueAsync (Explicit Transactions, RepeatableRead) ======]

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

        #endregion

        #region [====== EnqueueAsync & DequeueAsync (Explicit Transactions, ReadCommitted) ======]

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

        #endregion

        #region [====== EnqueueAsync & DequeueAsync (Explicit Transactions, ReadUncommitted) ======]

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

        #endregion

        #region [====== EnqueueAsync & DequeueAsync (Explicit Transactions) ======]

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
                var messages = CreateOutputMessages();

                using (CreateTransactionScope(isolationLevel))
                {
                    await queue.EnqueueAsync(messages);

                    Assert.AreEqual(messages.Length, await queue.CountAsync());
                }
                Assert.AreEqual(0, await queue.CountAsync());
            }
        }

        private async Task DequeueAsync_DequeuesExpectedMessages_IfMultipleSetsAreRemovedInTheSameTransaction_And_IsolationLevelIs(IsolationLevel isolationLevel)
        {
            const int batchSize = 5;

            await using (var queue = CreateMemoryQueue())
            {
                var messages = CreateOutputMessages(batchSize * 3);

                await queue.EnqueueAsync(messages);

                using (var scope = CreateTransactionScope(isolationLevel))
                {
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

        private async Task DequeueAsync_RollsbackChanges_IfTransactionIsAborted_And_IsolationLevelIs(IsolationLevel isolationLevel)
        {
            const int batchSize = 5;

            await using (var queue = CreateMemoryQueue())
            {
                var messages = CreateOutputMessages(batchSize * 3);

                await queue.EnqueueAsync(messages);

                using (CreateTransactionScope(isolationLevel))
                {
                    Assert.AreEqual(messages.Length, await queue.CountAsync());

                    var messagesA = await queue.DequeueAsync(batchSize);

                    Assert.AreEqual(batchSize, messagesA.Count);
                    Assert.AreEqual(messages.Length - batchSize, await queue.CountAsync());
                }
                Assert.AreEqual(messages.Length, await queue.CountAsync());
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
                var messages = CreateOutputMessages(batchSize * 3);

                using (CreateTransactionScope(isolationLevel))
                {
                    await queue.EnqueueAsync(messages);

                    Assert.AreEqual(messages.Length, await queue.CountAsync());

                    var messagesA = await queue.DequeueAsync(batchSize);

                    Assert.AreEqual(batchSize, messagesA.Count);
                    Assert.AreEqual(messages.Length - batchSize, await queue.CountAsync());

                    var messagesB = await queue.DequeueAsync(batchSize);

                    Assert.AreEqual(batchSize, messagesB.Count);
                    Assert.AreEqual(messages.Length - 2 * batchSize, await queue.CountAsync());
                }
                Assert.AreEqual(0, await queue.CountAsync());
            }
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
            new TransactionalMemoryQueue(new MessageSerializer(), TimeSpan.FromSeconds(120));

        private static IMessage[] CreateOutputMessages(int minimum = 1) =>
            CreateMessages(DateTimeOffset.UtcNow.Millisecond + minimum).ToArray();
    }
}
