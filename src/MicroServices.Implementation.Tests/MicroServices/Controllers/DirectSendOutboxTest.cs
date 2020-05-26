using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.MicroServices.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                await _outbox.SendAsync(CreateMessages(1));
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
                await _outbox.SendAsync(CreateMessages(1));
            }
            finally
            {
                _microServiceBus.AssertMessageCountIs(0);
            }
        }

        [TestMethod]
        public async Task SendAsync_SendsAllMessages_IfBothSenderAndReceiverHaveBeenStarted()
        {
            await _outbox.StartSendingMessagesAsync(CancellationToken.None);
            await _outbox.StartReceivingMessagesAsync(CancellationToken.None);

            var messageCount = DateTimeOffset.UtcNow.Millisecond;

            await _outbox.SendAsync(CreateMessages(messageCount));

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
                await _outbox.SendAsync(CreateMessages(1));
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
                await _outbox.SendAsync(CreateMessages(1));
            }
            finally
            {
                _microServiceBus.AssertMessageCountIs(0);
            }
        }

        #endregion

        #region [====== CreateMessages ======]

        private static readonly MessageFactory _MessageFactory = new MessagePipeline().BuildMessageFactory();

        private static IEnumerable<IMessage> CreateMessages(int count)
        {
            for (int index = 0; index < count; index++)
            {
                yield return CreateMessage(index);
            }
        }

        private static IMessage CreateMessage(object content) =>
            _MessageFactory.CreateEvent(MessageDirection.Output, MessageHeader.Unspecified, content);

        #endregion
    }
}
