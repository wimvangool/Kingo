using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceComponents.ComponentModel.Server;

namespace ServiceComponents.ComponentModel.Client
{
    [TestClass]
    public sealed class ClientEventBusTest
    {        
        private ClientEventBusStub _eventBus;

        [TestInitialize]
        public void Setup()
        {            
            _eventBus = new ClientEventBusStub();
        }        

        private IMessageHandler<object> MessageHandler
        {
            get { return _eventBus; }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Connect_Throws_IfSubscriberIsNull()
        {
            _eventBus.Connect(null);
        }

        [TestMethod]
        public void Connect_CreatesAndReturnsClosedConnection()
        {
            var subscriber = new object();
            var connection = _eventBus.Connect(subscriber);

            Assert.IsNotNull(connection);
            Assert.AreEqual(0, _eventBus.SubscriberCount);
            Assert.IsFalse(_eventBus.Contains(subscriber));
        }

        #region [====== Connection.Open ======]

        [TestMethod]
        public void ConnectionOpen_SubscribesSubscriber_IfConnectionIsClosed()
        {
            var subscriber = new object();
            var connection = _eventBus.Connect(subscriber);

            Assert.IsNotNull(connection);

            connection.Open();

            Assert.AreEqual(1, _eventBus.SubscriberCount);
            Assert.IsTrue(_eventBus.Contains(subscriber));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConnectionOpen_Throws_IfConnectionWasAlreadyOpen()
        {
            var subscriber = new object();
            var connection = _eventBus.Connect(subscriber, true);

            connection.Open();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ConnectionOpen_Throws_IfConnectionWasAlreadyDisposed()
        {
            var subscriber = new object();
            var connection = _eventBus.Connect(subscriber, true);

            connection.Dispose();
            connection.Open();
        }

        #endregion

        #region [====== Connection.Close ======]

        [TestMethod]
        public void ConnectionClose_UnsubscribesSubscriber_IfConnectionIsOpen()
        {
            var subscriber = new object();
            var connection = _eventBus.Connect(subscriber);

            Assert.IsNotNull(connection);

            connection.Open();
            connection.Close();

            Assert.AreEqual(0, _eventBus.SubscriberCount);
            Assert.IsFalse(_eventBus.Contains(subscriber));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConnectionClose_Throws_IfConnectionWasAlreadyClosed()
        {
            var subscriber = new object();
            var connection = _eventBus.Connect(subscriber);

            Assert.IsNotNull(connection);

            connection.Close();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ConnectionClose_Throws_IfConnectionWasAlreadyDisposed()
        {
            var subscriber = new object();
            var connection = _eventBus.Connect(subscriber, true);

            connection.Dispose();
            connection.Close();
        }

        #endregion

        #region [====== Connection.Dispose ======]

        [TestMethod]
        public void ConnectionDispose_UnsubscribesSubscriber_IfConnectionWasOpen()
        {
            var subscriber = new object();
            var connection = _eventBus.Connect(subscriber);

            Assert.IsNotNull(connection);

            connection.Open();
            connection.Dispose();

            Assert.AreEqual(0, _eventBus.SubscriberCount);
            Assert.IsFalse(_eventBus.Contains(subscriber));
        }

        [TestMethod]
        public void ConnectionDispose_DoesNothing_IfAlreadyDisposed()
        {
            var subscriber = new object();
            var connection = _eventBus.Connect(subscriber, true);

            connection.Dispose();
            connection.Dispose();
        }

        #endregion

        #region [====== Publish =====]

        [TestMethod]
        public void Publish_ImmediatelyPublishesMessage_IfNoTransactionIsActive()
        {
            MessageHandler.HandleAsync(new object());

            Assert.AreEqual(1, _eventBus.MessageCount);
        }        

        #endregion
    }
}
