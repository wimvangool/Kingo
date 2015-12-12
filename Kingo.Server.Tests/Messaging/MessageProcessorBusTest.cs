using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class MessageProcessorBusTest
    {
        private MessageProcessorBus _bus;

        [TestInitialize]
        public void Setup()
        {
            _bus = new MessageProcessorBus(new MessageProcessorStub());
        }        

        #region [====== ConnectGenericHandler ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConnectGenericHandler_Throws_IfHandlerIsNull()
        {           
            _bus.Connect(null as IMessageHandler<MessageOne>, false);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConnectGenericHandler_ReturnsClosedConnection_IfOpenConnectionIsFalse()
        {
            IMessageHandler<MessageOne> handler = new MessageHandlerSpy();

            using (var connection = _bus.Connect(handler, false))
            {
                Assert.IsNotNull(connection);

                connection.Close();
            }                                
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConnectGenericHandler_ReturnsOpenedConnection_IfOpenConnectionIsTrue()
        {            
            IMessageHandler<MessageOne> handler = new MessageHandlerSpy();

            using (var connection = _bus.Connect(handler, true))
            {
                Assert.IsNotNull(connection);

                connection.Open();
            }                      
        }

        [TestMethod]
        public void ConnectGenericHandler_ConnectsHandlerToBusForSpecificMessage()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.Connect<MessageOne>(handler, true))            
            {
                Assert.IsNotNull(connection);

                _bus.PublishAsync(new MessageOne()).Wait();
                _bus.PublishAsync(new MessageTwo()).Wait();

                Assert.AreEqual(1, handler.MessageOneCount);
                Assert.AreEqual(0, handler.MessageTwoCount);                
            }
        }

        [TestMethod]
        public void ConnectGenericHandler_CanBeCalledForDifferentMessageTypes()
        {
            var handler = new MessageHandlerSpy();

            using (var connectionA = _bus.Connect<MessageOne>(handler, true))
            using (var connectionB = _bus.Connect<MessageTwo>(handler, true))
            {
                Assert.IsNotNull(connectionA);
                Assert.IsNotNull(connectionB);

                _bus.PublishAsync(new MessageOne()).Wait();
                _bus.PublishAsync(new MessageTwo()).Wait();

                Assert.AreEqual(1, handler.MessageOneCount);
                Assert.AreEqual(1, handler.MessageTwoCount);
            }
        }

        [TestMethod]
        public void ConnectGenericHandler_DoesNotFowardMessages_IfConnectionIsNeverOpened()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.Connect<MessageOne>(handler, false))            
            {
                Assert.IsNotNull(connection);

                _bus.PublishAsync(new MessageOne()).Wait();

                Assert.AreEqual(0, handler.MessageOneCount);
                Assert.AreEqual(0, handler.MessageTwoCount);  
            }
        }

        [TestMethod]
        public void ConnectGenericHandler_StopsFowardingMessages_IfConnectionIsClosedAtSomePoint()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.Connect<MessageOne>(handler, true))            
            {
                Assert.IsNotNull(connection);

                _bus.PublishAsync(new MessageOne()).Wait();
                _bus.PublishAsync(new MessageOne()).Wait();                

                Assert.AreEqual(2, handler.MessageOneCount);
                Assert.AreEqual(0, handler.MessageTwoCount);

                connection.Close();

                _bus.PublishAsync(new MessageOne()).Wait();
                _bus.PublishAsync(new MessageOne()).Wait();                

                Assert.AreEqual(2, handler.MessageOneCount);
                Assert.AreEqual(0, handler.MessageTwoCount);
            }
        }

        #endregion

        #region [====== ConnectHandler ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConnectHandler_Throws_IfHandlerIsNull()
        {            
            _bus.Connect(null, false);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConnectHandler_ReturnsClosedConnection_IfOpenConnectionIsFalse()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.Connect(handler, false))
            {
                Assert.IsNotNull(connection);

                connection.Close();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConnectHandler_ReturnsOpenedConnection_IfOpenConnectionIsTrue()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.Connect(handler, true))
            {
                Assert.IsNotNull(connection);

                connection.Open();
            }
        }

        [TestMethod]
        public void ConnectHandler_ConnectsHandlerToBusForAllSupportedMessages()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.Connect(handler, true))            
            {
                Assert.IsNotNull(connection);

                _bus.PublishAsync(new MessageOne()).Wait();
                _bus.PublishAsync(new MessageTwo()).Wait();

                Assert.AreEqual(1, handler.MessageOneCount);
                Assert.AreEqual(1, handler.MessageTwoCount);      
            }
        }

        [TestMethod]
        public void ConnectHandler_DoesNotFowardMessages_IfConnectionIsNeverOpened()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.Connect(handler, false))            
            {
                Assert.IsNotNull(connection);

                _bus.PublishAsync(new MessageOne()).Wait();
                _bus.PublishAsync(new MessageTwo()).Wait();

                Assert.AreEqual(0, handler.MessageOneCount);
                Assert.AreEqual(0, handler.MessageTwoCount);      
            }
        }

        [TestMethod]
        public void ConnectHandler_StopsFowardingMessages_IfConnectionIsClosedAtSomePoint()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.Connect(handler, true))            
            {
                Assert.IsNotNull(connection);

                _bus.PublishAsync(new MessageOne()).Wait();
                _bus.PublishAsync(new MessageTwo()).Wait();

                Assert.AreEqual(1, handler.MessageOneCount);
                Assert.AreEqual(1, handler.MessageTwoCount);

                connection.Close();

                _bus.PublishAsync(new MessageOne()).Wait();
                _bus.PublishAsync(new MessageTwo()).Wait();

                Assert.AreEqual(1, handler.MessageOneCount);
                Assert.AreEqual(1, handler.MessageTwoCount);
            }
        }

        #endregion        
    }
}
