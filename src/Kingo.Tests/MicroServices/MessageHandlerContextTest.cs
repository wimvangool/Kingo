using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MessageHandlerContextTest
    {
        #region [====== Scoping ======]

        [TestMethod]
        public void Current_IsNull_IfCheckedOutsideAnyScope()
        {
            Assert.IsNull(MessageHandlerContext.Current);
        }

        [TestMethod]
        public void Current_IsNotNull_IfCheckedInsideScope()
        {
            using (CreateMessageHandlerContextScope())
            {
                Assert.IsNotNull(MessageHandlerContext.Current);
            }
        }

        [TestMethod]
        public void Current_IsUpdatedAsExpected_AsScopesAreCreatedAndDisposed()
        {
            Assert.IsNull(MicroProcessorContext.Current);            

            using (CreateMessageHandlerContextScope())
            {
                var context = MessageHandlerContext.Current;
                
                Assert.IsNotNull(context);

                using (CreateMessageHandlerContextScope(context.CreateContext(new object())))
                {
                    Assert.IsNotNull(MessageHandlerContext.Current);
                    Assert.AreNotSame(context, MessageHandlerContext.Current);
                }

                Assert.AreSame(context, MessageHandlerContext.Current);
            }

            Assert.IsNull(MicroProcessorContext.Current);
        }

        #endregion

        #region [====== Principal ======]

        [TestMethod]
        public void Principal_AlwaysReferencesProcessorPrincipal_AsScopesAreCreatedAndDisposed()
        {            
            using (CreateMessageHandlerContextScope())
            {                
                Assert.AreSame(Thread.CurrentPrincipal, MessageHandlerContext.Current.Principal);

                using (CreateMessageHandlerContextScope(MessageHandlerContext.Current.CreateContext(new object())))
                {                    
                    Assert.AreSame(Thread.CurrentPrincipal, MessageHandlerContext.Current.Principal);
                }

                Assert.AreSame(Thread.CurrentPrincipal, MessageHandlerContext.Current.Principal);
            }            
        }

        #endregion

        #region [====== Token ======]

        [TestMethod]
        public void Token_IsNone_IfSpecifiedTokenIsNull()
        {
            using (CreateMessageHandlerContextScope())
            {
                Assert.AreEqual(CancellationToken.None, MessageHandlerContext.Current.Token);
            }
        }

        [TestMethod]
        public void Token_IsSpecifiedToken_IfSpecifiedTokenIsNotNull()
        {
            var token = new CancellationToken();

            using (CreateMessageHandlerContextScope(token))
            {
                Assert.AreEqual(token, MessageHandlerContext.Current.Token);
            }
        }

        #endregion

        #region [====== Operation ======]        

        [TestMethod]
        public void Operation_ContainsExpectedMessage_IfOneMessageIsPushed()
        {                        
            var message = new object();            

            using (CreateMessageHandlerContextScope(null, message))
            {
                Assert.AreEqual("[InputStream] Object", MessageHandlerContext.Current.ToString());

                var operation = MessageHandlerContext.Current.Operation;
                
                Assert.IsNotNull(operation);
                Assert.AreSame(message, operation.Message);
                Assert.AreEqual(typeof(object), operation.MessageType);
                Assert.AreEqual(MicroProcessorOperationTypes.InputStream, operation.Type);

                Assert.AreEqual(1, operation.StackTrace().Count());                                                                
            }
        }

        [TestMethod]
        public void Operation_ContainsExpectedMessages_IfTwoMessagesArePushed()
        {                       
            var messageA = new object();
            var messageB = 10;

            using (CreateMessageHandlerContextScope(null, messageA))
            using (CreateMessageHandlerContextScope(MessageHandlerContext.Current.CreateContext(messageB)))
            {
                Assert.AreEqual("[InputStream] Object -> [OutputStream] Int32", MessageHandlerContext.Current.ToString());

                var operationB = MessageHandlerContext.Current.Operation;

                Assert.IsNotNull(operationB);
                Assert.AreEqual(messageB, operationB.Message);
                Assert.AreEqual(typeof(int), operationB.MessageType);
                Assert.AreEqual(MicroProcessorOperationTypes.OutputStream, operationB.Type);
                Assert.AreEqual(2, operationB.StackTrace().Count());                

                var operationA = operationB.PreviousOperation;

                Assert.IsNotNull(operationA);
                Assert.AreSame(messageA, operationA.Message);
                Assert.AreEqual(typeof(object), operationA.MessageType);
                Assert.AreEqual(MicroProcessorOperationTypes.InputStream, operationA.Type);
                Assert.AreEqual(1, operationA.StackTrace().Count());
            }
        }

        #endregion

        #region [====== UnitOfWork ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task EnlistAsync_Throws_IfResourceManagerIsNull()
        {
            using (CreateMessageHandlerContextScope())
            {
                await MessageHandlerContext.Current.UnitOfWork.EnlistAsync(null);
            }
        }

        [TestMethod]
        public async Task EnlistAsync_EnlistsTheSpecifiedResourceManager_IfResourceManagerIsNotNull()
        {
            var resourceManager = new UnitOfWorkResourceManagerSpy(false);

            using (CreateMessageHandlerContextScope())
            {
                await MessageHandlerContext.Current.UnitOfWork.EnlistAsync(resourceManager);
            }
            resourceManager.AssertRequiresFlushCountIs(0);
            resourceManager.AssertFlushCountIs(0);
        }                               

        [TestMethod]
        public async Task FlushAsync_DoesNotFlushResourceManager_IfResourceManagerDoesNotRequireFlush()
        {
            var resourceManager = new UnitOfWorkResourceManagerSpy(false);

            using (CreateMessageHandlerContextScope())
            {
                await MessageHandlerContext.Current.UnitOfWork.EnlistAsync(resourceManager);
                await MessageHandlerContext.Current.UnitOfWork.FlushAsync();
            }
            resourceManager.AssertRequiresFlushCountIs(1);
            resourceManager.AssertFlushCountIs(0);
        }

        [TestMethod]
        public async Task FlushAsync_FlushesResourceManager_IfResourceManagerRequiresFlush()
        {
            var resourceManager = new UnitOfWorkResourceManagerSpy(true);

            using (CreateMessageHandlerContextScope())
            {
                await MessageHandlerContext.Current.UnitOfWork.EnlistAsync(resourceManager);
                await MessageHandlerContext.Current.UnitOfWork.FlushAsync();
            }
            resourceManager.AssertRequiresFlushCountIs(1);
            resourceManager.AssertFlushCountIs(1);
        }        

        [TestMethod]        
        public async Task FlushAsync_DoesNothing_IfCalledMoreThanOnce()
        {
            using (CreateMessageHandlerContextScope())
            {
                await MessageHandlerContext.Current.UnitOfWork.FlushAsync();
                await MessageHandlerContext.Current.UnitOfWork.FlushAsync();
            }
        }

        #endregion

        private static IDisposable CreateMessageHandlerContextScope(CancellationToken? token = null, object message = null) =>
            CreateMessageHandlerContextScope(CreateMessageHandlerContext(token, message));

        private static IDisposable CreateMessageHandlerContextScope(MessageHandlerContext context) =>
            MicroProcessorContext.CreateScope(context);

        private static MessageHandlerContext CreateMessageHandlerContext(CancellationToken? token, object message) =>
            new MessageHandlerContext(ServiceProvider.Default, Thread.CurrentPrincipal, token, message ?? new object());
    }
}
