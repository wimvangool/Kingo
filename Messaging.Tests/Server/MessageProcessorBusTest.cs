using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace System.ComponentModel.Messaging.Server
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
            using (var waitHandle = new ManualResetEventSlim(false))
            {
                Assert.IsNotNull(connection);

                Task.Factory.StartNew(() =>
                {
                    _bus.Publish(new MessageOne());

                    waitHandle.Set();
                });

                _bus.Publish(new MessageOne());
                _bus.Publish(new MessageTwo());

                if (waitHandle.Wait(TimeSpan.FromSeconds(5)))
                {
                    Assert.AreEqual(2, handler.MessageOneCount);
                    Assert.AreEqual(0, handler.MessageTwoCount);
                    return;
                }
                Assert.Fail("Something went wrong with publishing the message asynchronously.");
            }
        }

        [TestMethod]
        public void ConnectGenericHandler_DoesNotFowardMessages_IfConnectionIsNeverOpened()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.Connect<MessageOne>(handler, false))
            using (var waitHandle = new ManualResetEventSlim(false))
            {
                Assert.IsNotNull(connection);

                Task.Factory.StartNew(() =>
                {
                    _bus.Publish(new MessageOne());

                    waitHandle.Set();
                });

                _bus.Publish(new MessageOne());
                _bus.Publish(new MessageTwo());

                if (waitHandle.Wait(TimeSpan.FromSeconds(5)))
                {
                    Assert.AreEqual(0, handler.MessageOneCount);
                    Assert.AreEqual(0, handler.MessageTwoCount);
                    return;
                }
                Assert.Fail("Something went wrong with publishing the message asynchronously.");
            }
        }

        [TestMethod]
        public void ConnectGenericHandler_StopsFowardingMessages_IfConnectionIsClosedAtSomePoint()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.Connect<MessageOne>(handler, true))
            using (var waitHandle = new ManualResetEventSlim(false))
            {
                Assert.IsNotNull(connection);

                Task.Factory.StartNew(() =>
                {
                    _bus.Publish(new MessageOne());

                    waitHandle.Set();
                });

                _bus.Publish(new MessageOne());
                _bus.Publish(new MessageTwo());                

                if (waitHandle.Wait(TimeSpan.FromSeconds(5)))
                {
                    connection.Close();

                    _bus.Publish(new MessageOne());

                    Assert.AreEqual(2, handler.MessageOneCount);
                    Assert.AreEqual(0, handler.MessageTwoCount);
                    return;
                }
                Assert.Fail("Something went wrong with publishing the message asynchronously.");
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
        public void ConnectHandler_ConnectsHandlerToBusForSpecificMessage()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.Connect(handler, true))
            using (var waitHandle = new ManualResetEventSlim(false))
            {
                Assert.IsNotNull(connection);

                Task.Factory.StartNew(() =>
                {
                    _bus.Publish(new MessageOne());

                    waitHandle.Set();
                });

                _bus.Publish(new MessageOne());
                _bus.Publish(new MessageTwo());

                if (waitHandle.Wait(TimeSpan.FromSeconds(5)))
                {
                    Assert.AreEqual(2, handler.MessageOneCount);
                    Assert.AreEqual(1, handler.MessageTwoCount);
                    return;
                }
                Assert.Fail("Something went wrong with publishing the message asynchronously.");
            }
        }

        [TestMethod]
        public void ConnectHandler_DoesNotFowardMessages_IfConnectionIsNeverOpened()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.Connect(handler, false))
            using (var waitHandle = new ManualResetEventSlim(false))
            {
                Assert.IsNotNull(connection);

                Task.Factory.StartNew(() =>
                {
                    _bus.Publish(new MessageOne());

                    waitHandle.Set();
                });

                _bus.Publish(new MessageOne());
                _bus.Publish(new MessageTwo());

                if (waitHandle.Wait(TimeSpan.FromSeconds(5)))
                {
                    Assert.AreEqual(0, handler.MessageOneCount);
                    Assert.AreEqual(0, handler.MessageTwoCount);
                    return;
                }
                Assert.Fail("Something went wrong with publishing the message asynchronously.");
            }
        }

        [TestMethod]
        public void ConnectHandler_StopsFowardingMessages_IfConnectionIsClosedAtSomePoint()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.Connect(handler, true))
            using (var waitHandle = new ManualResetEventSlim(false))
            {
                Assert.IsNotNull(connection);

                Task.Factory.StartNew(() =>
                {
                    _bus.Publish(new MessageOne());

                    waitHandle.Set();
                });

                _bus.Publish(new MessageOne());
                _bus.Publish(new MessageTwo());

                if (waitHandle.Wait(TimeSpan.FromSeconds(5)))
                {
                    connection.Close();

                    _bus.Publish(new MessageOne());

                    Assert.AreEqual(2, handler.MessageOneCount);
                    Assert.AreEqual(1, handler.MessageTwoCount);
                    return;
                }
                Assert.Fail("Something went wrong with publishing the message asynchronously.");
            }
        }

        #endregion

        #region [====== ConnectGenericHandlerThreadLocal ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConnectGenericHandlerThreadLocal_Throws_IfHandlerIsNull()
        {             
             _bus.ConnectThreadLocal(null as IMessageHandler<MessageOne>, false);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConnectGenericHandlerThreadLocal_ReturnsClosedConnection_IfOpenConnectionIsFalse()
        {
            IMessageHandler<MessageOne> handler = new MessageHandlerSpy();

            using (var connection = _bus.ConnectThreadLocal(handler, false))
            {
                Assert.IsNotNull(connection);

                connection.Close();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConnectGenericHandlerThreadLocal_ReturnsOpenedConnection_IfOpenConnectionIsTrue()
        {
            IMessageHandler<MessageOne> handler = new MessageHandlerSpy();

            using (var connection = _bus.ConnectThreadLocal(handler, true))
            {
                Assert.IsNotNull(connection);

                connection.Open();
            }
        }

        [TestMethod]
        public void ConnectGenericHandlerThreadLocal_ConnectsHandlerToBusForSpecificMessage()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.ConnectThreadLocal<MessageOne>(handler, true))
            using (var waitHandle = new ManualResetEventSlim(false))
            {
                Assert.IsNotNull(connection);

                Task.Factory.StartNew(() =>
                {
                    _bus.Publish(new MessageOne());

                    waitHandle.Set();
                });

                _bus.Publish(new MessageOne());
                _bus.Publish(new MessageTwo());

                if (waitHandle.Wait(TimeSpan.FromSeconds(5)))
                {
                    Assert.AreEqual(1, handler.MessageOneCount);
                    Assert.AreEqual(0, handler.MessageTwoCount);
                    return;
                }
                Assert.Fail("Something went wrong with publishing the message asynchronously.");
            }
        }

        [TestMethod]
        public void ConnectGenericHandlerThreadLocal_DoesNotFowardMessages_IfConnectionIsNeverOpened()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.ConnectThreadLocal<MessageOne>(handler, false))
            using (var waitHandle = new ManualResetEventSlim(false))
            {
                Assert.IsNotNull(connection);

                Task.Factory.StartNew(() =>
                {
                    _bus.Publish(new MessageOne());

                    waitHandle.Set();
                });

                _bus.Publish(new MessageOne());
                _bus.Publish(new MessageTwo());

                if (waitHandle.Wait(TimeSpan.FromSeconds(5)))
                {
                    Assert.AreEqual(0, handler.MessageOneCount);
                    Assert.AreEqual(0, handler.MessageTwoCount);
                    return;
                }
                Assert.Fail("Something went wrong with publishing the message asynchronously.");
            }
        }

        [TestMethod]
        public void ConnectGenericHandlerThreadLocal_StopsFowardingMessages_IfConnectionIsClosedAtSomePoint()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.ConnectThreadLocal<MessageOne>(handler, true))
            using (var waitHandle = new ManualResetEventSlim(false))
            {
                Assert.IsNotNull(connection);

                Task.Factory.StartNew(() =>
                {
                    _bus.Publish(new MessageOne());

                    waitHandle.Set();
                });

                _bus.Publish(new MessageOne());
                _bus.Publish(new MessageTwo());

                if (waitHandle.Wait(TimeSpan.FromSeconds(5)))
                {
                    connection.Close();

                    _bus.Publish(new MessageOne());

                    Assert.AreEqual(1, handler.MessageOneCount);
                    Assert.AreEqual(0, handler.MessageTwoCount);
                    return;
                }
                Assert.Fail("Something went wrong with publishing the message asynchronously.");
            }
        }

        #endregion

        #region [====== ConnectHandlerThreadLocal ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConnectHandlerThreadLocal_Throws_IfHandlerIsNull()
        {            
            _bus.ConnectThreadLocal(null, false);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConnectHandlerThreadLocal_ReturnsClosedConnection_IfOpenConnectionIsFalse()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.ConnectThreadLocal(handler, false))
            {
                Assert.IsNotNull(connection);

                connection.Close();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConnectHandlerThreadLocal_ReturnsOpenedConnection_IfOpenConnectionIsTrue()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.ConnectThreadLocal(handler, true))
            {
                Assert.IsNotNull(connection);

                connection.Open();
            }
        }

        [TestMethod]
        public void ConnectHandlerThreadLocal_ConnectsHandlerToBusForSpecificMessage()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.ConnectThreadLocal(handler, true))
            using (var waitHandle = new ManualResetEventSlim(false))
            {
                Assert.IsNotNull(connection);

                Task.Factory.StartNew(() =>
                {
                    _bus.Publish(new MessageOne());

                    waitHandle.Set();
                });

                _bus.Publish(new MessageOne());
                _bus.Publish(new MessageTwo());

                if (waitHandle.Wait(TimeSpan.FromSeconds(5)))
                {
                    Assert.AreEqual(1, handler.MessageOneCount);
                    Assert.AreEqual(1, handler.MessageTwoCount);
                    return;
                }
                Assert.Fail("Something went wrong with publishing the message asynchronously.");
            }
        }

        [TestMethod]
        public void ConnectHandlerThreadLocal_DoesNotFowardMessages_IfConnectionIsNeverOpened()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.ConnectThreadLocal(handler, false))
            using (var waitHandle = new ManualResetEventSlim(false))
            {
                Assert.IsNotNull(connection);

                Task.Factory.StartNew(() =>
                {
                    _bus.Publish(new MessageOne());

                    waitHandle.Set();
                });

                _bus.Publish(new MessageOne());
                _bus.Publish(new MessageTwo());

                if (waitHandle.Wait(TimeSpan.FromSeconds(5)))
                {
                    Assert.AreEqual(0, handler.MessageOneCount);
                    Assert.AreEqual(0, handler.MessageTwoCount);
                    return;
                }
                Assert.Fail("Something went wrong with publishing the message asynchronously.");
            }
        }

        [TestMethod]
        public void ConnectHandlerThreadLocal_StopsFowardingMessages_IfConnectionIsClosedAtSomePoint()
        {
            var handler = new MessageHandlerSpy();

            using (var connection = _bus.ConnectThreadLocal(handler, true))
            using (var waitHandle = new ManualResetEventSlim(false))
            {
                Assert.IsNotNull(connection);

                Task.Factory.StartNew(() =>
                {
                    _bus.Publish(new MessageOne());

                    waitHandle.Set();
                });

                _bus.Publish(new MessageOne());
                _bus.Publish(new MessageTwo());

                if (waitHandle.Wait(TimeSpan.FromSeconds(5)))
                {
                    connection.Close();

                    _bus.Publish(new MessageOne());

                    Assert.AreEqual(1, handler.MessageOneCount);
                    Assert.AreEqual(1, handler.MessageTwoCount);
                    return;
                }
                Assert.Fail("Something went wrong with publishing the message asynchronously.");
            }
        }

        #endregion
    }
}
