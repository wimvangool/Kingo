using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class MicroServiceBusTest
    {
        #region [====== Stub ======]

        private sealed class MicroServiceBusStub : MicroServiceBus
        {
            private readonly MicroServiceBusClientStub _clientStub;

            public MicroServiceBusStub() : base(MessageDirection.Output, MessageDirection.Output)
            {
                _clientStub = new MicroServiceBusClientStub();
            }

            protected override Task<MicroServiceBusClient> CreateSenderAsync(CancellationToken token) =>
                CreateProxyAsync();

            protected override Task<MicroServiceBusClient> CreateReceiverAsync(CancellationToken token) =>
                CreateProxyAsync();

            private Task<MicroServiceBusClient> CreateProxyAsync() =>
                Task.FromResult<MicroServiceBusClient>(_clientStub);
        }

        private sealed class MicroServiceBusClientStub : MicroServiceBusClient
        {
            protected override Task SendAsync(IMessage[] messages) =>
                Task.CompletedTask;

            protected override TransactionScope CreateTransactionScope() =>
                new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
        }

        #endregion

        private const string _Sender = "Sender";
        private const string _Receiver = "Receiver";

        private const string _StoppedState = "Stopped";
        private const string _StartedState = "Started";

        #region [====== StartAsync(...), StopAsync(...) & DisposeAsync(...) ======]

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
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task StartSendingMessagesAsync_Throws_IfBusHasAlreadyBeenDisposed()
        {
            var bus = new MicroServiceBusStub();

            bus.Dispose();
            await bus.StartSendingMessagesAsync(CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task StartSendingMessagesAsync_Throws_IfBusHasAlreadyBeenDisposedAsynchronously()
        {
            var bus = new MicroServiceBusStub();

            await bus.DisposeAsync();
            await bus.StartSendingMessagesAsync(CancellationToken.None);
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
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task StartReceivingMessagesAsync_Throws_IfBusHasAlreadyBeenDisposed()
        {
            var bus = new MicroServiceBusStub();

            bus.Dispose();
            await bus.StartReceivingMessagesAsync(CancellationToken.None);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task StartReceivingMessagesAsync_Throws_IfBusHasAlreadyBeenDisposedAsynchronously()
        {
            var bus = new MicroServiceBusStub();

            await bus.DisposeAsync();
            await bus.StartReceivingMessagesAsync(CancellationToken.None);
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
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task StopSendingMessagesAsync_Throws_IfBusHasAlreadyBeenDisposed()
        {
            var bus = new MicroServiceBusStub();

            bus.Dispose();
            await bus.StopSendingMessagesAsync();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task StopSendingMessagesAsync_Throws_IfBusHasAlreadyBeenDisposedAsynchronously()
        {
            var bus = new MicroServiceBusStub();

            await bus.DisposeAsync();
            await bus.StopSendingMessagesAsync();
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

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task StopReceivingMessagesAsync_Throws_IfBusHasAlreadyBeenDisposed()
        {
            var bus = new MicroServiceBusStub();

            bus.Dispose();
            await bus.StopReceivingMessagesAsync();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task StopReceivingMessagesAsync_Throws_IfBusHasAlreadyBeenDisposedAsynchronously()
        {
            var bus = new MicroServiceBusStub();

            await bus.DisposeAsync();
            await bus.StopReceivingMessagesAsync();
        }

        private static void AssertIsInState(object queue, string component, string state) =>
            Assert.IsTrue(queue.ToString().Contains($"{component} = {state}"), $"{component} is not in state '{state}'.");

        #endregion
    }
}
