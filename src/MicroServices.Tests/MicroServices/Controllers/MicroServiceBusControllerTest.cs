using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class MicroServiceBusControllerTest
    {
        #region [====== MicroServiceBusControllerStub ======]

        private sealed class MicroServiceBusControllerStub<TMessage> : MicroServiceBusController where TMessage : class
        {
            private readonly MicroServiceBusControllerTest _test;
            private readonly CancellationTokenSource _tokenSource;
            private int _disposeCount;

            public MicroServiceBusControllerStub(IMicroProcessor processor, MicroServiceBusControllerTest test, CancellationTokenSource tokenSource = null) :
                base(processor)
            {
                _test = test;
                _tokenSource = tokenSource;
            }

            protected override Task<IMicroServiceBusClient> CreateClientAsync()
            {
                _tokenSource?.Cancel();
                return Task.FromResult(_test.CreateClient<TMessage>());
            }

            protected override TypeSet DefineServiceContract(TypeSet serviceContract) =>
                serviceContract.Add<TMessage>();

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
                Assert.AreEqual($"{nameof(MicroServiceBusControllerStub<TMessage>)}<{typeof(TMessage).FriendlyName()}> ({state})", ToString());
        }

        #endregion

        #region [====== MicroServiceBusClientStub ======]

        private sealed class MicroServiceBusClientStub<TMessage> : MicroServiceBusClient<TMessage>, IMicroServiceBusClientStub where TMessage : class
        {
            private int _sendCount;
            private int _publishCount;
            private int _disposeCount;

            protected override Task SendAsync(TMessage command)
            {
                Interlocked.Increment(ref _sendCount);
                return Task.CompletedTask;
            }

            protected override Task PublishAsync(TMessage @event)
            {
                Interlocked.Increment(ref _publishCount);
                return Task.CompletedTask;
            }

            public void AssertSendCountIs(int count) =>
                Assert.AreEqual(count, _sendCount);

            public void AssertPublishCountIs(int count) =>
                Assert.AreEqual(count, _publishCount);

            protected override TMessage Pack(IMessageToDispatch message) =>
                (TMessage) message.Content;

            protected override IMessageEnvelope Unpack(TMessage message, IMessageEnvelopeBuilder messageBuilder) =>
                messageBuilder.Wrap(message);

            protected override void Dispose(bool disposing)
            {
                Interlocked.Increment(ref _disposeCount);
                base.Dispose(disposing);
            }

            public void AssertDisposeCountIs(int count) =>
                Assert.AreEqual(count, _disposeCount);
        }

        private interface IMicroServiceBusClientStub : IMicroServiceBusClient
        {
            void AssertSendCountIs(int count);

            void AssertPublishCountIs(int count);

            void AssertDisposeCountIs(int count);
        }

        #endregion

        private readonly Mock<IMicroProcessorServiceProvider> _serviceProviderMock;
        private readonly Mock<IMicroProcessor> _processorMock;
        private readonly List<IMicroServiceBusClientStub> _clientStubs;

        public MicroServiceBusControllerTest()
        {
            _serviceProviderMock = new Mock<IMicroProcessorServiceProvider>();
            _processorMock = new Mock<IMicroProcessor>();
            _processorMock.Setup(processor => processor.ServiceProvider).Returns(_serviceProviderMock.Object);
            _clientStubs = new List<IMicroServiceBusClientStub>();
        }

        private IMicroProcessor Processor =>
            _processorMock.Object;

        private IMicroServiceBusClient CreateClient<TMessage>() where TMessage : class
        {
            var client = new MicroServiceBusClientStub<TMessage>();
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

        #region [====== SendAsync ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SendAsync_Throws_IfControllerHasNotBeenStartedYet()
        {
            var controller = CreateController();

            try
            {
                await controller.SendCommandsAsync(0);
            }
            catch (InvalidOperationException)
            {
                Assert.AreEqual(0, _clientStubs.Count);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SendAsync_Throws_IfStartOfControllerWasCancelledBeforeFullyConnected()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                var controller = CreateController(tokenSource);

                await controller.StartAsync(tokenSource.Token);

                try
                {
                    await controller.SendCommandsAsync(0);
                }
                catch (InvalidOperationException)
                {
                    Assert.AreEqual(1, _clientStubs.Count);

                    var client = _clientStubs[0];
                    client.AssertSendCountIs(0);
                    client.AssertDisposeCountIs(1);
                    throw;
                }
            }
        }

        [TestMethod]
        public async Task SendAsync_DoesNotSendTheSpecifiedCommand_IfControllerHasBeenStarted_But_CommandIsNotPartOfServiceContract()
        {
            var controller = CreateController();

            await controller.StartAsync(CancellationToken.None);
            await controller.SendCommandsAsync(0);

            Assert.AreEqual(1, _clientStubs.Count);

            var client = _clientStubs[0];
            client.AssertSendCountIs(0);
            client.AssertDisposeCountIs(0);
        }

        [TestMethod]
        public async Task SendAsync_SendsTheSpecifiedCommand_IfControllerHasBeenStarted_And_CommandIsPartOfServiceContract()
        {
            var controller = CreateController();

            await controller.StartAsync(CancellationToken.None);
            await controller.SendCommandsAsync(new object());

            Assert.AreEqual(1, _clientStubs.Count);

            var client = _clientStubs[0];
            client.AssertSendCountIs(1);
            client.AssertDisposeCountIs(0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task SendAsync_Throws_IfControllerHasNotBeenStoppedAgain()
        {
            var controller = CreateController();

            await controller.StartAsync(CancellationToken.None);
            await controller.StopAsync(CancellationToken.None);

            try
            {
                await controller.SendCommandsAsync(0);
            }
            catch (InvalidOperationException)
            {
                Assert.AreEqual(1, _clientStubs.Count);

                var client = _clientStubs[0];
                client.AssertSendCountIs(0);
                client.AssertDisposeCountIs(1);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public async Task SendAsync_Throws_IfControllerHasBeenDisposed()
        {
            var controller = CreateController();

            await controller.StartAsync(CancellationToken.None);

            controller.Dispose();

            try
            {
                await controller.SendCommandsAsync(0);
            }
            catch (ObjectDisposedException)
            {
                Assert.AreEqual(1, _clientStubs.Count);

                var client = _clientStubs[0];
                client.AssertSendCountIs(0);
                client.AssertDisposeCountIs(1);
                throw;
            }
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
                await controller.PublishEventsAsync(0);
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
                    await controller.PublishEventsAsync(0);
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
        public async Task PublishAsync_DoesNotPublishEvent_IfControllerHasBeenStarted_But_EventIsNotPartOfTheServiceContract()
        {
            var controller = CreateController();

            await controller.StartAsync(CancellationToken.None);
            await controller.PublishEventsAsync(0);

            Assert.AreEqual(1, _clientStubs.Count);

            var client = _clientStubs[0];
            client.AssertPublishCountIs(0);
            client.AssertDisposeCountIs(0);
        }

        [TestMethod]
        public async Task PublishAsync_PublishesTheSpecifiedEvent_IfControllerHasBeenStarted_And_EventIsPartOfTheServiceContract()
        {
            var controller = CreateController();

            await controller.StartAsync(CancellationToken.None);
            await controller.PublishEventsAsync(new object());

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
                await controller.PublishEventsAsync(0);
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
                await controller.PublishEventsAsync(0);
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

        private MicroServiceBusControllerStub<object> CreateController(CancellationTokenSource tokenSource = null) =>
            CreateController<object>(tokenSource);

        private MicroServiceBusControllerStub<TMessage> CreateController<TMessage>(CancellationTokenSource tokenSource = null) where TMessage : class
        {
            var controller = new MicroServiceBusControllerStub<TMessage>(Processor, this, tokenSource);

            _serviceProviderMock.Setup(provider => provider.GetService(typeof(IEnumerable<MicroServiceBusController>))).Returns(new MicroServiceBusController[]
            {
                controller
            });

            return controller;
        }
    }
}
