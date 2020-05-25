using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.MicroServices.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class ForwardOnlyQueueTest
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
        private readonly ForwardOnlyQueue _queue;

        public ForwardOnlyQueueTest()
        {
            _queue = new ForwardOnlyQueue(_microServiceBus = new MicroServiceBusStub());
        }

        #region [====== SendAsync ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SendAsync_Throws_IfSenderHasNotBeenStarted()
        {
            try
            {
                await _queue.SendAsync(CreateMessages(1));
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
            await _queue.StartSendingMessagesAsync(CancellationToken.None);

            try
            {
                await _queue.SendAsync(CreateMessages(1));
            }
            finally
            {
                _microServiceBus.AssertMessageCountIs(0);
            }
        }

        [TestMethod]
        public async Task SendAsync_SendsAllMessages_IfBothSenderAndReceiverHaveBeenStarted()
        {
            await _queue.StartSendingMessagesAsync(CancellationToken.None);
            await _queue.StartReceivingMessagesAsync(CancellationToken.None);

            var messageCount = DateTimeOffset.UtcNow.Millisecond;

            await _queue.SendAsync(CreateMessages(messageCount));

            _microServiceBus.AssertMessageCountIs(messageCount);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SendAsync_Throws_IfReceiverHasBeenStoppedAgain()
        {
            await _queue.StartSendingMessagesAsync(CancellationToken.None);
            await _queue.StartReceivingMessagesAsync(CancellationToken.None);
            await _queue.StopReceivingMessagesAsync();

            try
            {
                await _queue.SendAsync(CreateMessages(1));
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
            await _queue.StartSendingMessagesAsync(CancellationToken.None);
            await _queue.StartReceivingMessagesAsync(CancellationToken.None);
            await _queue.StopSendingMessagesAsync();

            try
            {
                await _queue.SendAsync(CreateMessages(1));
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
