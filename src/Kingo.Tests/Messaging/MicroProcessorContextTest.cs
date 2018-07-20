using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class MicroProcessorContextTest
    {
        #region [====== Token ======]

        [TestMethod]
        public void Token_IsNone_IfSpecifiedTokenIsNull()
        {
            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                Assert.AreEqual(CancellationToken.None, scope.Context.Token);
            }
        }

        [TestMethod]
        public void Token_IsSpecifiedToken_IfSpecifiedTokenIsNotNull()
        {
            var token = new CancellationToken();

            using (var scope = CreateScope(new MessageHandlerContext(Principal, token)))
            {
                Assert.AreEqual(token, scope.Context.Token);
            }
        }

        #endregion

        #region [====== Messages ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MessageStackIndexer_Throws_IfIndexIsNegative()
        {
            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                scope.Context.StackTrace[-1].IgnoreValue();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MessageStackIndexer_Throws_IfIndexIsZero_And_NoMessageHasBeenPushed()
        {
            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                scope.Context.StackTrace[0].IgnoreValue();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MessageStackIndexer_Throws_IfIndexIsGreaterThanZero()
        {
            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                scope.Context.StackTrace[1].IgnoreValue();
            }
        }

        [TestMethod]
        public void MessageStack_ContainsExpectedMessage_IfOneMessageIsPushed()
        {
            const string expectedStringValue = "[InputStream] System.Object";

            var context = new MessageHandlerContext(Principal);
            var message = new object();

            using (var scope = CreateScope(context))
            {
                context.StackTraceCore.Push(new MicroProcessorOperation(MicroProcessorOperationTypes.InputStream, message));

                Assert.IsNotNull(scope.Context.StackTrace);
                Assert.AreEqual(1, scope.Context.StackTrace.Count);
                Assert.AreEqual(1, scope.Context.StackTrace.Count());

                Assert.IsNotNull(scope.Context.StackTrace.CurrentOperation);
                Assert.AreSame(message, scope.Context.StackTrace.CurrentOperation.Message);
                Assert.AreSame(message, scope.Context.StackTrace[0].Message);

                Assert.AreEqual(expectedStringValue, scope.Context.StackTrace.ToString());
                Assert.AreEqual(expectedStringValue, scope.Context.ToString());                                
            }
        }

        [TestMethod]
        public void MessageStack_ContainsExpectedMessages_IfTwoMessagesArePushed()
        {
            const string expectedStringValue = "[InputStream] System.Object -> [OutputStream] 10";

            var context = new MessageHandlerContext(Principal);
            var messageA = new object();
            int messageB = 10;

            using (var scope = CreateScope(context))
            {
                context.StackTraceCore.Push(new MicroProcessorOperation(MicroProcessorOperationTypes.InputStream, messageA));
                context.StackTraceCore.Push(new MicroProcessorOperation(MicroProcessorOperationTypes.OutputStream, messageB));

                Assert.IsNotNull(scope.Context.StackTrace);
                Assert.AreEqual(2, scope.Context.StackTrace.Count);
                Assert.AreEqual(2, scope.Context.StackTrace.Count());

                Assert.IsNotNull(scope.Context.StackTrace.CurrentOperation);
                Assert.AreEqual(messageB, scope.Context.StackTrace.CurrentOperation.Message);
                Assert.AreEqual(messageB, scope.Context.StackTrace[0].Message);
                Assert.AreSame(messageA, scope.Context.StackTrace[1].Message);

                Assert.AreEqual(expectedStringValue, scope.Context.StackTrace.ToString());
                Assert.AreEqual(expectedStringValue, scope.Context.ToString());
            }
        }

        #endregion

        #region [====== EnlistAsync ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnlistAsync_Throws_IfUnitOfWorkIsNull()
        {
            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                scope.Context.UnitOfWork.EnlistAsync(null);
            }
        }

        [TestMethod]
        public async Task EnlistAsync_EnlistsTheSpecifiedUnitOfWork_IfUnitOfWorkIsNotNull()
        {
            var unitOfWork = new UnitOfWorkSpy(false);

            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                await scope.Context.UnitOfWork.EnlistAsync(unitOfWork);
            }
            unitOfWork.AssertRequiresFlushCountIs(0);
            unitOfWork.AssertFlushCountIs(0);
        }               

        [TestMethod]
        public async Task EnlistAsync_FlushesUnitOfWorkOnlyOnce_IfUnitOfWorkIsEnlistedTwice_And_ResourceIdsAreEqual()
        {
            var unitOfWork = new UnitOfWorkSpy(true);

            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                await scope.Context.UnitOfWork.EnlistAsync(unitOfWork);
                await scope.Context.UnitOfWork.EnlistAsync(unitOfWork);
                await scope.Context.UnitOfWork.FlushAsync();
            }
            unitOfWork.AssertRequiresFlushCountIs(1);
            unitOfWork.AssertFlushCountIs(1);
        }

        [TestMethod]
        public async Task EnlistAsync_FlushesUnitOfWorkTwice_IfUnitOfWorkIsEnlistedTwice_And_ResourceIdsAreNotEqual()
        {
            var unitOfWork = new UnitOfWorkSpy(true);

            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                await scope.Context.UnitOfWork.EnlistAsync(unitOfWork);
                await scope.Context.UnitOfWork.EnlistAsync(unitOfWork, new object());
                await scope.Context.UnitOfWork.FlushAsync();
            }
            unitOfWork.AssertRequiresFlushCountIs(2);
            unitOfWork.AssertFlushCountIs(2);
        }

        #endregion

        #region [====== FlushAsync ======]

        [TestMethod]
        public async Task FlushAsync_DoesNotFlushUnitOfWork_IfUnitOfWorkDoesNotRequireFlush()
        {
            var unitOfWork = new UnitOfWorkSpy(false);

            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                await scope.Context.UnitOfWork.EnlistAsync(unitOfWork);
                await scope.Context.UnitOfWork.FlushAsync();
            }
            unitOfWork.AssertRequiresFlushCountIs(1);
            unitOfWork.AssertFlushCountIs(0);
        }

        [TestMethod]
        public async Task FlushAsync_FlushesUnitOfWork_IfUnitOfWorkRequiresFlush()
        {
            var unitOfWork = new UnitOfWorkSpy(true);

            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                await scope.Context.UnitOfWork.EnlistAsync(unitOfWork);
                await scope.Context.UnitOfWork.FlushAsync();
            }
            unitOfWork.AssertRequiresFlushCountIs(1);
            unitOfWork.AssertFlushCountIs(1);
        }        

        [TestMethod]        
        public async Task FlushAsync_DoesNothing_IfCalledMoreThanOnce()
        {
            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                await scope.Context.UnitOfWork.FlushAsync();
                await scope.Context.UnitOfWork.FlushAsync();
            }
        }

        #endregion

        [TestMethod]
        public void PreviousContext_IsRestored_AfterScopeHasBeenDisposed()
        {
            using (CreateScope(new MessageHandlerContext(Principal))) { }

            Assert.IsTrue(MicroProcessorContext.Current.StackTrace.IsEmpty);
        }        

        private static IPrincipal Principal =>
            Thread.CurrentPrincipal;        

        private static MicroProcessorContextScope CreateScope(MicroProcessorContext context) =>
            MicroProcessorContext.CreateScope(context);
    }
}
