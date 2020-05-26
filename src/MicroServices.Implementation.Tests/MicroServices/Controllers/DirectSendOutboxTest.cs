using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

            public MicroServiceBusStub()
            {
                _messages = new List<IMessage>();
            }

            public Task SendAsync(IEnumerable<IMessage> messages)
            {
                _messages.AddRange(messages);
                return Task.CompletedTask;
            }

            public void AssertMessageCountIs(int count) =>
                Assert.AreEqual(count, _messages.Count);
        }

        #endregion

        private readonly MicroServiceBusStub _microServiceBus;
        private readonly DirectSendOutbox _outbox;

        public DirectSendOutboxTest()
        {
            _outbox = new DirectSendOutbox(_microServiceBus = new MicroServiceBusStub());
        }

        #region [====== SendAsync ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SendAsync_Throws_IfSenderHasNotBeenStarted()
        {
            try
            {
                await _outbox.SendAsync(CreateInt32Messages(1));
            }
            finally
            {
                _microServiceBus.AssertMessageCountIs(0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SendAsync_Throws_IfReceiverHasNotBeenStarted()
        {
            await _outbox.StartSendingMessagesAsync(CancellationToken.None);

            try
            {
                await _outbox.SendAsync(CreateInt32Messages(1));
            }
            finally
            {
                _microServiceBus.AssertMessageCountIs(0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SendAsync_Throws_IfBothSenderAndReceiverHaveBeenStarted_But_MessagesIsNull()
        {
            await _outbox.StartSendingMessagesAsync(CancellationToken.None);
            await _outbox.StartReceivingMessagesAsync(CancellationToken.None);

            await _outbox.SendAsync(null);
        }

        [TestMethod]
        public async Task SendAsync_SendsSomeMessages_IfBothSenderAndReceiverHaveBeenStarted_But_SomeMessagesHaveUnsupportedDirection()
        {
            await _outbox.StartSendingMessagesAsync(CancellationToken.None);
            await _outbox.StartReceivingMessagesAsync(CancellationToken.None);

            var messageCount = DateTimeOffset.UtcNow.Millisecond + 1;
            var messages = CreateInt32Messages(messageCount).Concat(CreateInt32Messages(messageCount, MessageKind.Event, MessageDirection.Input));

            await _outbox.SendAsync(messages);

            _microServiceBus.AssertMessageCountIs(messageCount);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SendAsync_Throws_IfReceiverHasBeenStoppedAgain()
        {
            await _outbox.StartSendingMessagesAsync(CancellationToken.None);
            await _outbox.StartReceivingMessagesAsync(CancellationToken.None);
            await _outbox.StopReceivingMessagesAsync();

            try
            {
                await _outbox.SendAsync(CreateInt32Messages(1));
            }
            finally
            {
                _microServiceBus.AssertMessageCountIs(0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SendAsync_Throws_IfSenderHasBeenStoppedAgain()
        {
            await _outbox.StartSendingMessagesAsync(CancellationToken.None);
            await _outbox.StartReceivingMessagesAsync(CancellationToken.None);
            await _outbox.StopSendingMessagesAsync();

            try
            {
                await _outbox.SendAsync(CreateInt32Messages(1));
            }
            finally
            {
                _microServiceBus.AssertMessageCountIs(0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task SendAsync_Throws_IfOutboxHasBeenDisposed()
        {
            await _outbox.StartSendingMessagesAsync(CancellationToken.None);
            _outbox.Dispose();

            try
            {
                await _outbox.SendAsync(CreateInt32Messages(1));
            }
            finally
            {
                _microServiceBus.AssertMessageCountIs(0);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task SendAsync_Throws_IfOutboxHasBeenDisposedAsynchronously()
        {
            await _outbox.StartSendingMessagesAsync(CancellationToken.None);
            await _outbox.DisposeAsync();

            try
            {
                await _outbox.SendAsync(CreateInt32Messages(1));
            }
            finally
            {
                _microServiceBus.AssertMessageCountIs(0);
            }
        }

        #endregion
    }
}
