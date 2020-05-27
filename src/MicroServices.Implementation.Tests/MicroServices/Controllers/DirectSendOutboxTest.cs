using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Kingo.MicroServices.MessageFactoryTest;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class DirectSendOutboxTest
    {
        #region [====== Stubs ======]

        private sealed class MicroServiceBusStub : IMicroServiceBus
        {
            private readonly List<IMessage> _messages;
            private Transaction _transaction;

            public MicroServiceBusStub()
            {
                _messages = new List<IMessage>();
            }

            public Transaction Transaction =>
                _transaction;

            public Task SendAsync(IEnumerable<IMessage> messages)
            {
                _transaction = Transaction.Current;
                _messages.AddRange(messages);
                return Task.CompletedTask;
            }

            public void AssertMessageCountIs(int count) =>
                Assert.AreEqual(count, _messages.Count);
        }

        #endregion

        private readonly MicroServiceBusStub _microServiceBus;

        public DirectSendOutboxTest()
        {
            _microServiceBus = new MicroServiceBusStub();
        }

        #region [====== SendAsync (Start, Stop & Dispose) ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SendAsync_Throws_IfSenderHasNotBeenStarted()
        {
            await using (var outbox = new DirectSendOutbox(_microServiceBus))
            {
                try
                {
                    await outbox.SendAsync(CreateInt32Messages(1));
                }
                finally
                {
                    _microServiceBus.AssertMessageCountIs(0);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SendAsync_Throws_IfReceiverHasNotBeenStarted()
        {
            await using (var outbox = new DirectSendOutbox(_microServiceBus))
            {
                await outbox.StartSendingMessagesAsync(CancellationToken.None);

                try
                {
                    await outbox.SendAsync(CreateInt32Messages(1));
                }
                finally
                {
                    _microServiceBus.AssertMessageCountIs(0);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SendAsync_Throws_IfBothSenderAndReceiverHaveBeenStarted_But_MessagesIsNull()
        {
            await using (var outbox = new DirectSendOutbox(_microServiceBus))
            {
                await outbox.StartSendingMessagesAsync(CancellationToken.None);
                await outbox.StartReceivingMessagesAsync(CancellationToken.None);

                await outbox.SendAsync(null);
            }
        }

        [TestMethod]
        public async Task SendAsync_SendsSomeMessages_IfBothSenderAndReceiverHaveBeenStarted_But_SomeMessagesHaveUnsupportedDirection()
        {
            await using (var outbox = new DirectSendOutbox(_microServiceBus))
            {
                await outbox.StartSendingMessagesAsync(CancellationToken.None);
                await outbox.StartReceivingMessagesAsync(CancellationToken.None);

                var messageCount = DateTimeOffset.UtcNow.Millisecond + 1;
                var messages = CreateInt32Messages(messageCount).Concat(CreateInt32Messages(messageCount, MessageKind.Event, MessageDirection.Input));

                await outbox.SendAsync(messages);

                _microServiceBus.AssertMessageCountIs(messageCount);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SendAsync_Throws_IfReceiverHasBeenStoppedAgain()
        {
            await using (var outbox = new DirectSendOutbox(_microServiceBus))
            {
                await outbox.StartSendingMessagesAsync(CancellationToken.None);
                await outbox.StartReceivingMessagesAsync(CancellationToken.None);
                await outbox.StopReceivingMessagesAsync();

                try
                {
                    await outbox.SendAsync(CreateInt32Messages(1));
                }
                finally
                {
                    _microServiceBus.AssertMessageCountIs(0);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SendAsync_Throws_IfSenderHasBeenStoppedAgain()
        {
            await using (var outbox = new DirectSendOutbox(_microServiceBus))
            {
                await outbox.StartSendingMessagesAsync(CancellationToken.None);
                await outbox.StartReceivingMessagesAsync(CancellationToken.None);
                await outbox.StopSendingMessagesAsync();

                try
                {
                    await outbox.SendAsync(CreateInt32Messages(1));
                }
                finally
                {
                    _microServiceBus.AssertMessageCountIs(0);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task SendAsync_Throws_IfOutboxHasBeenDisposed()
        {
            await using (var outbox = new DirectSendOutbox(_microServiceBus))
            {
                await outbox.StartSendingMessagesAsync(CancellationToken.None);
                outbox.Dispose();

                try
                {
                    await outbox.SendAsync(CreateInt32Messages(1));
                }
                finally
                {
                    _microServiceBus.AssertMessageCountIs(0);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task SendAsync_Throws_IfOutboxHasBeenDisposedAsynchronously()
        {
            await using (var outbox = new DirectSendOutbox(_microServiceBus))
            {
                await outbox.StartSendingMessagesAsync(CancellationToken.None);
                await outbox.DisposeAsync();

                try
                {
                    await outbox.SendAsync(CreateInt32Messages(1));
                }
                finally
                {
                    _microServiceBus.AssertMessageCountIs(0);
                }
            }
        }

        #endregion

        #region [====== SendAsync (Transactions) ======]

        [TestMethod]
        public async Task SendAsync_CreatesNewTransaction_IfTransactionScopeOptionIsRequired_And_CurrentTransactionIsNull()
        {
            await using (var outbox = new DirectSendOutbox(_microServiceBus))
            {
                await outbox.StartSendingMessagesAsync(CancellationToken.None);
                await outbox.StartReceivingMessagesAsync(CancellationToken.None);

                await outbox.SendAsync(CreateInt32Messages(DateTimeOffset.UtcNow.Millisecond));

                Assert.IsNotNull(_microServiceBus.Transaction);
            }
        }

        [TestMethod]
        public async Task SendAsync_ExecutesWithinExistingTransaction_IfTransactionScopeOptionIsRequired_And_CurrentTransactionIsNotNull()
        {
            await using (var outbox = new DirectSendOutbox(_microServiceBus))
            {
                await outbox.StartSendingMessagesAsync(CancellationToken.None);
                await outbox.StartReceivingMessagesAsync(CancellationToken.None);

                using (var scope = new TransactionScope())
                {
                    await outbox.SendAsync(CreateInt32Messages(DateTimeOffset.UtcNow.Millisecond));

                    Assert.AreSame(Transaction.Current, _microServiceBus.Transaction);
                    scope.Complete();
                }
            }
        }

        [TestMethod]
        public async Task SendAsync_CreatesNewTransaction_IfTransactionScopeOptionIsRequiresNew_And_CurrentTransactionIsNull()
        {
            await using (var outbox = new DirectSendOutbox(_microServiceBus, TransactionScopeOption.RequiresNew))
            {
                await outbox.StartSendingMessagesAsync(CancellationToken.None);
                await outbox.StartReceivingMessagesAsync(CancellationToken.None);

                await outbox.SendAsync(CreateInt32Messages(DateTimeOffset.UtcNow.Millisecond));

                Assert.IsNotNull(_microServiceBus.Transaction);
            }
        }

        [TestMethod]
        public async Task SendAsync_CreatesNewTransaction_IfTransactionScopeOptionIsRequiresNew_And_CurrentTransactionIsNotNull()
        {
            await using (var outbox = new DirectSendOutbox(_microServiceBus, TransactionScopeOption.RequiresNew))
            {
                await outbox.StartSendingMessagesAsync(CancellationToken.None);
                await outbox.StartReceivingMessagesAsync(CancellationToken.None);

                using (var scope = new TransactionScope())
                {
                    await outbox.SendAsync(CreateInt32Messages(DateTimeOffset.UtcNow.Millisecond));

                    Assert.IsNotNull(_microServiceBus.Transaction);
                    Assert.AreNotSame(Transaction.Current, _microServiceBus.Transaction);
                    scope.Complete();
                }
            }
        }

        [TestMethod]
        public async Task SendAsync_ExecutesWithinNoTransaction_IfTransactionScopeOptionIsSuppress_And_CurrentTransactionIsNull()
        {
            await using (var outbox = new DirectSendOutbox(_microServiceBus, TransactionScopeOption.Suppress))
            {
                await outbox.StartSendingMessagesAsync(CancellationToken.None);
                await outbox.StartReceivingMessagesAsync(CancellationToken.None);

                await outbox.SendAsync(CreateInt32Messages(DateTimeOffset.UtcNow.Millisecond));

                Assert.IsNull(_microServiceBus.Transaction);
            }
        }

        [TestMethod]
        public async Task SendAsync_ExecutesWithinNoTransaction_IfTransactionScopeOptionIsSuppress_And_CurrentTransactionIsNotNull()
        {
            await using (var outbox = new DirectSendOutbox(_microServiceBus, TransactionScopeOption.Suppress))
            {
                await outbox.StartSendingMessagesAsync(CancellationToken.None);
                await outbox.StartReceivingMessagesAsync(CancellationToken.None);

                using (var scope = new TransactionScope())
                {
                    await outbox.SendAsync(CreateInt32Messages(DateTimeOffset.UtcNow.Millisecond));

                    Assert.IsNull(_microServiceBus.Transaction);
                    scope.Complete();
                }
            }
        }

        #endregion
    }
}
