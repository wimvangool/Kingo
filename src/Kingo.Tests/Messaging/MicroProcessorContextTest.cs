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
            using (CreateScope(new MessageHandlerContext(Principal)))
            {
                Assert.AreEqual(CancellationToken.None, CurrentContext.Token);
            }
        }

        [TestMethod]
        public void Token_IsSpecifiedToken_IfSpecifiedTokenIsNotNull()
        {
            var token = new CancellationToken();

            using (CreateScope(new MessageHandlerContext(Principal, token)))
            {
                Assert.AreEqual(token, CurrentContext.Token);
            }
        }

        #endregion

        #region [====== Messages ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MessageStackIndexer_Throws_IfIndexIsNegative()
        {
            using (CreateScope(new MessageHandlerContext(Principal)))
            {
                CurrentContext.StackTrace[-1].IgnoreValue();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MessageStackIndexer_Throws_IfIndexIsZero_And_NoMessageHasBeenPushed()
        {
            using (CreateScope(new MessageHandlerContext(Principal)))
            {
                CurrentContext.StackTrace[0].IgnoreValue();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MessageStackIndexer_Throws_IfIndexIsGreaterThanZero()
        {
            using (CreateScope(new MessageHandlerContext(Principal)))
            {
                CurrentContext.StackTrace[1].IgnoreValue();
            }
        }

        [TestMethod]
        public void MessageStack_ContainsExpectedMessage_IfOneMessageIsPushed()
        {
            const string expectedStringValue = "[InputStream] System.Object";

            var context = new MessageHandlerContext(Principal);
            var message = new object();

            using (CreateScope(context))
            {
                context.StackTraceCore.Push(MessageInfo.FromInputStream(message));

                Assert.IsNotNull(CurrentContext.StackTrace);
                Assert.AreEqual(1, CurrentContext.StackTrace.Count);
                Assert.AreEqual(1, CurrentContext.StackTrace.Count());

                Assert.IsNotNull(CurrentContext.StackTrace.Current);
                Assert.AreSame(message, CurrentContext.StackTrace.Current.Message);
                Assert.AreSame(message, CurrentContext.StackTrace[0].Message);

                Assert.AreEqual(expectedStringValue, CurrentContext.StackTrace.ToString());
                Assert.AreEqual(expectedStringValue, CurrentContext.ToString());                                
            }
        }

        [TestMethod]
        public void MessageStack_ContainsExpectedMessages_IfTwoMessagesArePushed()
        {
            const string expectedStringValue = "[InputStream] System.Object -> [OutputStream] 10";

            var context = new MessageHandlerContext(Principal);
            var messageA = new object();
            int messageB = 10;

            using (CreateScope(context))
            {
                context.StackTraceCore.Push(MessageInfo.FromInputStream(messageA));
                context.StackTraceCore.Push(MessageInfo.FromOutputStream(messageB));

                Assert.IsNotNull(CurrentContext.StackTrace);
                Assert.AreEqual(2, CurrentContext.StackTrace.Count);
                Assert.AreEqual(2, CurrentContext.StackTrace.Count());

                Assert.IsNotNull(CurrentContext.StackTrace.Current);
                Assert.AreEqual(messageB, CurrentContext.StackTrace.Current.Message);
                Assert.AreEqual(messageB, CurrentContext.StackTrace[0].Message);
                Assert.AreSame(messageA, CurrentContext.StackTrace[1].Message);

                Assert.AreEqual(expectedStringValue, CurrentContext.StackTrace.ToString());
                Assert.AreEqual(expectedStringValue, CurrentContext.ToString());
            }
        }

        #endregion

        #region [====== EnlistAsync ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnlistAsync_Throws_IfUnitOfWorkIsNull()
        {
            using (CreateScope(new MessageHandlerContext(Principal)))
            {
                CurrentContext.UnitOfWork.EnlistAsync(null);
            }
        }

        [TestMethod]
        public async Task EnlistAsync_EnlistsTheSpecifiedUnitOfWork_IfUnitOfWorkIsNotNull()
        {
            var unitOfWork = new UnitOfWorkSpy(false);

            using (CreateScope(new MessageHandlerContext(Principal)))
            {
                await CurrentContext.UnitOfWork.EnlistAsync(unitOfWork);
            }
            unitOfWork.AssertRequiresFlushCountIs(0);
            unitOfWork.AssertFlushCountIs(0);
        }

        [TestMethod]
        public async Task EnlistAsync_DoesNotFlushUnitOfWork_IfContextIsAlreadyFlushing_And_UnitOfWorkDoesNotRequireFlush()
        {
            var unitOfWork = new UnitOfWorkSpy(false);

            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                await scope.CompleteAsync();
                await CurrentContext.UnitOfWork.EnlistAsync(unitOfWork);
            }
            unitOfWork.AssertRequiresFlushCountIs(1);
            unitOfWork.AssertFlushCountIs(0);
        }

        [TestMethod]
        public async Task EnlistAsync_FlushesUnitOfWork_IfContextIsAlreadyFlushing_And_UnitOfWorkRequiresFlush()
        {
            var unitOfWork = new UnitOfWorkSpy(true);

            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                await scope.CompleteAsync();
                await CurrentContext.UnitOfWork.EnlistAsync(unitOfWork);
            }
            unitOfWork.AssertRequiresFlushCountIs(1);
            unitOfWork.AssertFlushCountIs(1);
        }

        [TestMethod]
        public async Task EnlistAsync_FlushesUnitOfWorkOnlyOnce_IfUnitOfWorkIsEnlistedTwice_And_ResourceIdsAreEqual()
        {
            var unitOfWork = new UnitOfWorkSpy(true);

            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                await CurrentContext.UnitOfWork.EnlistAsync(unitOfWork);
                await CurrentContext.UnitOfWork.EnlistAsync(unitOfWork);
                await scope.CompleteAsync();                
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
                await CurrentContext.UnitOfWork.EnlistAsync(unitOfWork);
                await CurrentContext.UnitOfWork.EnlistAsync(unitOfWork, new object());
                await scope.CompleteAsync();
            }
            unitOfWork.AssertRequiresFlushCountIs(2);
            unitOfWork.AssertFlushCountIs(2);
        }

        #endregion

        #region [====== CompleteAsync ======]

        [TestMethod]
        public async Task CompleteAsync_DoesNotFlushUnitOfWork_IfUnitOfWorkDoesNotRequireFlush()
        {
            var unitOfWork = new UnitOfWorkSpy(false);

            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                await CurrentContext.UnitOfWork.EnlistAsync(unitOfWork);
                await scope.CompleteAsync();
            }
            unitOfWork.AssertRequiresFlushCountIs(1);
            unitOfWork.AssertFlushCountIs(0);
        }

        [TestMethod]
        public async Task CompleteAsync_FlushesUnitOfWork_IfUnitOfWorkRequiresFlush()
        {
            var unitOfWork = new UnitOfWorkSpy(true);

            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                await CurrentContext.UnitOfWork.EnlistAsync(unitOfWork);
                await scope.CompleteAsync();
            }
            unitOfWork.AssertRequiresFlushCountIs(1);
            unitOfWork.AssertFlushCountIs(1);
        }        

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public async Task CompleteAsync_DoesNothing_IfCalledMoreThanOnce()
        {
            using (var scope = CreateScope(new MessageHandlerContext(Principal)))
            {
                await scope.CompleteAsync();
                await scope.CompleteAsync();
            }
        }

        #endregion

        [TestMethod]
        public void PreviousContext_IsRestored_AfterScopeHasBeenDisposed()
        {
            using (CreateScope(new MessageHandlerContext(Principal))) { }

            Assert.AreEqual(0, CurrentContext.StackTrace.Count);
        }        

        private static IPrincipal Principal =>
            Thread.CurrentPrincipal;

        private static IMicroProcessorContext CurrentContext =>
            MicroProcessorContext.Current;

        private static MicroProcessorContextScope CreateScope(MicroProcessorContext context) =>
            MicroProcessorContext.CreateScope(context);
    }
}
