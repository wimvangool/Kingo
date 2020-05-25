using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class MicroServiceBusTest
    {
        #region [====== Stub ======]

        private sealed class MicroServiceBusStub : MicroServiceBus
        {
            private readonly MicroServiceBusProxyStub _proxyStub;

            public MicroServiceBusStub()
            {
                _proxyStub = new MicroServiceBusProxyStub();
            }

            protected override Task<MicroServiceBusProxy> StartSenderAsync(CancellationToken token) =>
                CreateProxyAsync();

            protected override Task<MicroServiceBusProxy> StartReceiverAsync(CancellationToken token) =>
                CreateProxyAsync();

            private Task<MicroServiceBusProxy> CreateProxyAsync() =>
                Task.FromResult<MicroServiceBusProxy>(_proxyStub);
        }

        private sealed class MicroServiceBusProxyStub : MicroServiceBusProxy
        {
            public override Task SendAsync(IEnumerable<IMessage> messages) =>
                Task.CompletedTask;
        }

        #endregion

        private const string _Sender = "Sender";
        private const string _Receiver = "Receiver";

        private const string _StoppedState = "Stopped";
        private const string _StartedState = "Started";

        #region [====== Start & Stop ======]

        [TestMethod]
        public async Task StartSendingMessagesAsync_StartsTheSender_IfSenderHasNotBeenStarted()
        {
            await using (var bus = new MicroServiceBusStub())
            {
                await bus.StartSendingMessagesAsync(CancellationToken.None);

                AssertIsInState(bus, _Sender, _StartedState);
                AssertIsInState(bus, _Receiver, _StoppedState);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task StartSendingMessagesAsync_Throws_IfSenderHasAlreadyBeenStarted()
        {
            await using (var bus = new MicroServiceBusStub())
            {
                await bus.StartSendingMessagesAsync(CancellationToken.None);

                try
                {
                    await bus.StartSendingMessagesAsync(CancellationToken.None);
                }
                finally
                {
                    AssertIsInState(bus, _Sender, _StartedState);
                    AssertIsInState(bus, _Receiver, _StoppedState);
                }
            }
        }

        [TestMethod]
        public async Task StartReceivingMessagesAsync_StartsTheReceiver_IfReceiverHasNotBeenStarted()
        {
            await using (var bus = new MicroServiceBusStub())
            {
                await bus.StartReceivingMessagesAsync(CancellationToken.None);

                AssertIsInState(bus, _Sender, _StoppedState);
                AssertIsInState(bus, _Receiver, _StartedState);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task StartReceivingMessagesAsync_Throws_IfReceiverHasAlreadyBeenStarted()
        {
            await using (var bus = new MicroServiceBusStub())
            {
                await bus.StartReceivingMessagesAsync(CancellationToken.None);

                try
                {
                    await bus.StartReceivingMessagesAsync(CancellationToken.None);
                }
                finally
                {
                    AssertIsInState(bus, _Sender, _StoppedState);
                    AssertIsInState(bus, _Receiver, _StartedState);
                }
            }
        }

        [TestMethod]
        public async Task StopSendingMessagesAsync_DoesNothing_IfSenderHasNotBeenStarted()
        {
            await using (var bus = new MicroServiceBusStub())
            {
                await bus.StopSendingMessagesAsync();

                AssertIsInState(bus, _Sender, _StoppedState);
                AssertIsInState(bus, _Receiver, _StoppedState);
            }
        }

        [TestMethod]
        public async Task StopSendingMessagesAsync_StopsTheSender_IfSenderHasBeenStarted()
        {
            await using (var bus = new MicroServiceBusStub())
            {
                await bus.StartSendingMessagesAsync(CancellationToken.None);
                await bus.StopSendingMessagesAsync();

                AssertIsInState(bus, _Sender, _StoppedState);
                AssertIsInState(bus, _Receiver, _StoppedState);
            }
        }

        [TestMethod]
        public async Task StopReceivingMessagesAsync_DoesNothing_IfReceiverHasNotBeenStarted()
        {
            await using (var bus = new MicroServiceBusStub())
            {
                await bus.StopReceivingMessagesAsync();

                AssertIsInState(bus, _Sender, _StoppedState);
                AssertIsInState(bus, _Receiver, _StoppedState);
            }
        }

        [TestMethod]
        public async Task StopReceivingMessagesAsync_StopsTheReceiver_IfReceiverHasBeenStarted()
        {
            await using (var bus = new MicroServiceBusStub())
            {
                await bus.StartReceivingMessagesAsync(CancellationToken.None);
                await bus.StopReceivingMessagesAsync();

                AssertIsInState(bus, _Sender, _StoppedState);
                AssertIsInState(bus, _Receiver, _StoppedState);
            }
        }

        private static void AssertIsInState(object queue, string component, string state) =>
            Assert.IsTrue(queue.ToString().Contains($"{component} = {state}"), $"{component} is not in state '{state}'.");

        #endregion
    }
}
