using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class MicroServiceBusControllerTest
    {
        #region [====== MicroServiceBusControllerStub ======]

        private sealed class MicroServiceBusControllerStub : MicroServiceBusController
        {
            private readonly MicroServiceBusControllerTest _test;
            private readonly CancellationTokenSource _tokenSource;
            private int _disposeCount;

            public MicroServiceBusControllerStub(IMicroServiceBusProcessor processor, MicroServiceBusControllerTest test, CancellationTokenSource tokenSource = null) :
                base(processor, test.Bus)
            {
                _test = test;
                _tokenSource = tokenSource;
            }

            protected override Task<IMicroServiceBusClient> CreateClientAsync(IMicroServiceBus bus)
            {
                _tokenSource?.Cancel();
                return Task.FromResult(_test.CreateClient(bus));
            }

            protected override void Dispose(bool disposing)
            {
                Interlocked.Increment(ref _disposeCount);
                base.Dispose(disposing);
            }

            public void AssertDisposeCountIs(int count) =>
                Assert.AreEqual(count, _disposeCount);

            public void AssertInStoppedState() =>
                AssertState("Stopped");

            public void AssertInStartedState() =>
                AssertState("Started");

            public void AssertInDisposedState() =>
                AssertState("Disposed");

            private void AssertState(string state) =>
                Assert.AreEqual($"{nameof(MicroServiceBusControllerStub)} ({state})", ToString());
        }

        #endregion

        #region [====== MicroServiceBusClientStub ======]

        private sealed class MicroServiceBusClientStub : MicroServiceBusClient<int>
        {
            private int _publishCount;
            private int _disposeCount;

            public MicroServiceBusClientStub(IMicroServiceBus bus)
            {
                Bus = bus;
            }

            protected override IMicroServiceBus Bus
            {
                get;
            }

            protected override Task PublishAsync(int @event)
            {
                Interlocked.Increment(ref _publishCount);
                return Task.CompletedTask;
            }

            public void AssertPublishCountIs(int count) =>
                Assert.AreEqual(count, _publishCount);

            protected override int Pack(object message) =>
                (int) message;

            protected override object Unpack(int message) =>
                message;

            protected override void Dispose(bool disposing)
            {
                Interlocked.Increment(ref _disposeCount);
                base.Dispose(disposing);
            }

            public void AssertDisposeCountIs(int count) =>
                Assert.AreEqual(count, _disposeCount);
        }

        #endregion

        private readonly Mock<IMicroServiceBusProcessor> _processorMock;
        private readonly Mock<IMicroServiceBus> _busMock;
        private readonly List<MicroServiceBusClientStub> _clientStubs;

        public MicroServiceBusControllerTest()
        {
            _processorMock = new Mock<IMicroServiceBusProcessor>();
            _busMock = new Mock<IMicroServiceBus>();
            _clientStubs = new List<MicroServiceBusClientStub>();
        }

        private IMicroServiceBusProcessor Processor =>
            _processorMock.Object;

        private IMicroServiceBus Bus =>
            _busMock.Object;

        private IMicroServiceBusClient CreateClient(IMicroServiceBus bus)
        {
            var client = new MicroServiceBusClientStub(bus);
            _clientStubs.Add(client);
            return client;
        }

        #region [====== StartAsync ======]

        [TestMethod]
        public async Task StartAsync_StartsTheControllerAndCreatesTheClient_IfCalledTheFirstTime()
        {
            var controller = CreateController();

            await controller.StartAsync(CancellationToken.None);

            controller.AssertInStartedState();
            controller.AssertDisposeCountIs(0);

            Assert.AreEqual(1, _clientStubs.Count);

            var client = _clientStubs[0];
            client.AssertPublishCountIs(0);
            client.AssertDisposeCountIs(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task StartAsync_Throws_IfControllerIsAlreadyStarted()
        {
            var controller = CreateController();

            try
            {
                await Task.WhenAll(controller.StartAsync(CancellationToken.None), controller.StartAsync(CancellationToken.None));
            }
            catch (InvalidOperationException)
            {
                controller.AssertInStartedState();
                controller.AssertDisposeCountIs(0);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task StartAsync_Throws_IfControllerIsAlreadyDisposed()
        {
            var controller = CreateController();

            controller.Dispose();

            try
            {
                await controller.StartAsync(CancellationToken.None);
            }
            catch (ObjectDisposedException)
            {
                controller.AssertInDisposedState();
                controller.AssertDisposeCountIs(1);

                Assert.AreEqual(0, _clientStubs.Count);
                throw;
            }
        }

        [TestMethod]
        public async Task StartAsync_StartsTheControllerAndCreatesTheClient_IfCalledAgainAfterControllerHasBeenStopped()
        {
            var controller = CreateController();

            await controller.StartAsync(CancellationToken.None);
            await controller.StopAsync(CancellationToken.None);
            await controller.StartAsync(CancellationToken.None);

            controller.AssertInStartedState();
            controller.AssertDisposeCountIs(0);

            Assert.AreEqual(2, _clientStubs.Count);

            var clientOne = _clientStubs[0];
            clientOne.AssertPublishCountIs(0);
            clientOne.AssertDisposeCountIs(1);

            var clientTwo = _clientStubs[1];
            clientTwo.AssertPublishCountIs(0);
            clientTwo.AssertDisposeCountIs(0);
        }

        [TestMethod]
        public async Task StartAsync_DisposesCreatedClient_IfStartIsCancelledBeforeClientIsFullyConnected()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                var controller = CreateController(tokenSource);

                await controller.StartAsync(tokenSource.Token);

                controller.AssertInStoppedState();
                controller.AssertDisposeCountIs(0);

                Assert.AreEqual(1, _clientStubs.Count);

                var client = _clientStubs[0];
                client.AssertPublishCountIs(0);
                client.AssertDisposeCountIs(1);
            }
        }

        #endregion

        #region [====== StopAsync ======]

        [TestMethod]
        public async Task StopAsync_DoesNothing_IfControllerIsAlreadyStopped()
        {
            var controller = CreateController();

            await controller.StopAsync(CancellationToken.None);

            controller.AssertInStoppedState();
            controller.AssertDisposeCountIs(0);

            Assert.AreEqual(0, _clientStubs.Count);
        }

        [TestMethod]
        public async Task StopAsync_StopsTheControllerAndDisposesTheClient_IfControllerIsStarted()
        {
            var controller = CreateController();

            await controller.StartAsync(CancellationToken.None);
            await controller.StopAsync(CancellationToken.None);

            controller.AssertInStoppedState();
            controller.AssertDisposeCountIs(0);

            Assert.AreEqual(1, _clientStubs.Count);

            var client = _clientStubs[0];
            client.AssertPublishCountIs(0);
            client.AssertDisposeCountIs(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task StopAsync_Throws_IfControllerIsAlreadyDisposed()
        {
            var controller = CreateController();

            controller.Dispose();

            try
            {
                await controller.StopAsync(CancellationToken.None);
            }
            catch (ObjectDisposedException)
            {
                controller.AssertInDisposedState();
                controller.AssertDisposeCountIs(1);

                Assert.AreEqual(0, _clientStubs.Count);
                throw;
            }
        }

        #endregion

        #region [====== Dispose ======]

        [TestMethod]
        public void Dispose_DisposesTheController_IfControllerHasNotBeenStartedYet()
        {
            var controller = CreateController();

            controller.Dispose();

            controller.AssertInDisposedState();
            controller.AssertDisposeCountIs(1);

            Assert.AreEqual(0, _clientStubs.Count);
        }

        [TestMethod]
        public void Dispose_DoesNothing_IfControllerHasAlreadyBeenDisposed()
        {
            var controller = CreateController();

            controller.Dispose();
            controller.Dispose();

            controller.AssertInDisposedState();
            controller.AssertDisposeCountIs(1);

            Assert.AreEqual(0, _clientStubs.Count);
        }

        [TestMethod]
        public async Task Dispose_DisposesTheControllerAndTheClient_IfControllerHasAlreadyBeenStarted()
        {
            var controller = CreateController();

            await controller.StartAsync(CancellationToken.None);

            controller.Dispose();

            controller.AssertInDisposedState();
            controller.AssertDisposeCountIs(1);

            Assert.AreEqual(1, _clientStubs.Count);

            var client = _clientStubs[0];
            client.AssertPublishCountIs(0);
            client.AssertDisposeCountIs(1);
        }

        [TestMethod]
        public async Task Dispose_DisposesTheController_IfControllerHasAlreadyBeenStartedAndStopped()
        {
            var controller = CreateController();

            await controller.StartAsync(CancellationToken.None);
            await controller.StopAsync(CancellationToken.None);

            controller.Dispose();

            controller.AssertInDisposedState();
            controller.AssertDisposeCountIs(1);

            Assert.AreEqual(1, _clientStubs.Count);

            var client = _clientStubs[0];
            client.AssertPublishCountIs(0);
            client.AssertDisposeCountIs(1);
        }

        #endregion

        #region [====== PublishAsync ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task PublishAsync_Throws_IfControllerHasNotBeenStartedYet()
        {
            var controller = CreateController();

            try
            {
                await controller.PublishAsync(0);
            }
            catch (InvalidOperationException)
            {
                Assert.AreEqual(0, _clientStubs.Count);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task PublishAsync_Throws_IfStartOfControllerWasCancelledBeforeFullyConnected()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                var controller = CreateController(tokenSource);

                await controller.StartAsync(tokenSource.Token);

                try
                {
                    await controller.PublishAsync(0);
                }
                catch (InvalidOperationException)
                {
                    Assert.AreEqual(1, _clientStubs.Count);

                    var client = _clientStubs[0];
                    client.AssertPublishCountIs(0);
                    client.AssertDisposeCountIs(1);
                    throw;
                }
            }
        }

        [TestMethod]
        public async Task PublishAsync_PublishesTheSpecifiedEvent_IfControllerHasBeenStarted()
        {
            var controller = CreateController();

            await controller.StartAsync(CancellationToken.None);
            await controller.PublishAsync(0);

            Assert.AreEqual(1, _clientStubs.Count);

            var client = _clientStubs[0];
            client.AssertPublishCountIs(1);
            client.AssertDisposeCountIs(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task PublishAsync_Throws_IfControllerHasNotBeenStoppedAgain()
        {
            var controller = CreateController();

            await controller.StartAsync(CancellationToken.None);
            await controller.StopAsync(CancellationToken.None);

            try
            {
                await controller.PublishAsync(0);
            }
            catch (InvalidOperationException)
            {
                Assert.AreEqual(1, _clientStubs.Count);

                var client = _clientStubs[0];
                client.AssertPublishCountIs(0);
                client.AssertDisposeCountIs(1);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task PublishAsync_Throws_IfControllerHasBeenDisposed()
        {
            var controller = CreateController();

            await controller.StartAsync(CancellationToken.None);
            
            controller.Dispose();

            try
            {
                await controller.PublishAsync(0);
            }
            catch (ObjectDisposedException)
            {
                Assert.AreEqual(1, _clientStubs.Count);

                var client = _clientStubs[0];
                client.AssertPublishCountIs(0);
                client.AssertDisposeCountIs(1);
                throw;
            }
        }

        #endregion

        private MicroServiceBusControllerStub CreateController(CancellationTokenSource tokenSource = null) =>
            new MicroServiceBusControllerStub(Processor, this, tokenSource);
    }
}
