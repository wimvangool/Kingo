using System;
using System.Linq;
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
            using (CreateScope(new MessageHandlerContext()))
            {
                Assert.AreEqual(CancellationToken.None, CurrentContext.Token);
            }
        }

        [TestMethod]
        public void Token_IsSpecifiedToken_IfSpecifiedTokenIsNotNull()
        {
            var token = new CancellationToken();

            using (CreateScope(new MessageHandlerContext(token)))
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
            using (CreateScope(new MessageHandlerContext()))
            {
                CurrentContext.Messages[-1].IgnoreValue();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MessageStackIndexer_Throws_IfIndexIsZero_And_NoMessageHasBeenPushed()
        {
            using (CreateScope(new MessageHandlerContext()))
            {
                CurrentContext.Messages[0].IgnoreValue();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MessageStackIndexer_Throws_IfIndexIsGreaterThanZero()
        {
            using (CreateScope(new MessageHandlerContext()))
            {
                CurrentContext.Messages[1].IgnoreValue();
            }
        }

        [TestMethod]
        public void MessageStack_ContainsExpectedMessage_IfOneMessageIsPushed()
        {
            const string expectedStringValue = "Object (InputStream)";

            var context = new MessageHandlerContext();
            var message = new object();

            using (CreateScope(context))
            {
                context.Messages.Push(MessageInfo.FromInputStream(message));

                Assert.IsNotNull(CurrentContext.Messages);
                Assert.AreEqual(1, CurrentContext.Messages.Count);
                Assert.AreEqual(1, CurrentContext.Messages.Count());

                Assert.IsNotNull(CurrentContext.Messages.Current);
                Assert.AreSame(message, CurrentContext.Messages.Current.Message);
                Assert.AreSame(message, CurrentContext.Messages[0].Message);

                Assert.AreEqual(expectedStringValue, CurrentContext.Messages.ToString());
                Assert.AreEqual(expectedStringValue, CurrentContext.ToString());                                
            }
        }

        [TestMethod]
        public void MessageStack_ContainsExpectedMessages_IfTwoMessagesArePushed()
        {
            const string expectedStringValue = "Object (InputStream) -> Int32 (OutputStream)";

            var context = new MessageHandlerContext();
            var messageA = new object();
            int messageB = 10;

            using (var scope = CreateScope(context))
            {
                context.Messages.Push(MessageInfo.FromInputStream(messageA));
                context.Messages.Push(MessageInfo.FromOutputStream(messageB));

                Assert.IsNotNull(CurrentContext.Messages);
                Assert.AreEqual(2, CurrentContext.Messages.Count);
                Assert.AreEqual(2, CurrentContext.Messages.Count());

                Assert.IsNotNull(CurrentContext.Messages.Current);
                Assert.AreEqual(messageB, CurrentContext.Messages.Current.Message);
                Assert.AreEqual(messageB, CurrentContext.Messages[0].Message);
                Assert.AreSame(messageA, CurrentContext.Messages[1].Message);

                Assert.AreEqual(expectedStringValue, CurrentContext.Messages.ToString());
                Assert.AreEqual(expectedStringValue, CurrentContext.ToString());
            }
        }

        #endregion

        #region [====== EnlistAsync ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnlistAsync_Throws_IfUnitOfWorkIsNull()
        {
            using (CreateScope(new MessageHandlerContext()))
            {
                CurrentContext.UnitOfWork.EnlistAsync(null);
            }
        }

        [TestMethod]
        public async Task EnlistAsync_EnlistsTheSpecifiedUnitOfWork_IfUnitOfWorkIsNotNull()
        {
            var unitOfWork = new UnitOfWorkSpy(false);

            using (CreateScope(new MessageHandlerContext()))
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

            using (var scope = CreateScope(new MessageHandlerContext()))
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

            using (var scope = CreateScope(new MessageHandlerContext()))
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

            using (var scope = CreateScope(new MessageHandlerContext()))
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

            using (var scope = CreateScope(new MessageHandlerContext()))
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

            using (var scope = CreateScope(new MessageHandlerContext()))
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

            using (var scope = CreateScope(new MessageHandlerContext()))
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
            using (var scope = CreateScope(new MessageHandlerContext()))
            {
                await scope.CompleteAsync();
                await scope.CompleteAsync();
            }
        }

        #endregion

        [TestMethod]
        public void PreviousContext_IsRestored_AfterScopeHasBeenDisposed()
        {
            using (CreateScope(new MessageHandlerContext())) { }

            Assert.AreEqual(0, CurrentContext.Messages.Count);
        }

        [TestMethod]
        public async Task CreateScope_CanSafelyBeUsedInsideAnotherThread_AsLongAsRelativeDisposeOrderIsCorrect()
        {
            var outerContext = new MessageHandlerContext();

            using (var waitHandleOuter = new AutoResetEvent(false))
            using (var waitHandleInner = new AutoResetEvent(false))
            using (CreateScope(outerContext))
            {
                outerContext.Messages.Push(MessageInfo.FromInputStream(1));

                var innerContext = outerContext.CreateMetadataContext();
                var innerTask = Task.Run(async () =>
                {
                    Assert.AreEqual(1, CurrentContext.Messages.Current.Message);

                    using (CreateScope(innerContext))
                    {
                        Assert.AreEqual(1, CurrentContext.Messages.Count);
                        Assert.AreEqual(1, CurrentContext.Messages.Current.Message);

                        // After the inner scope is created on this thread, we signal the outer thread to continue and wait for a signal.
                        waitHandleOuter.Set();
                        waitHandleInner.WaitOne();

                        // When we got a signal, the outer thread has pushed another message to its stack, which should be invisible to us.
                        Assert.AreEqual(1, CurrentContext.Messages.Count);
                        Assert.AreEqual(1, CurrentContext.Messages.Current.Message);

                        innerContext.Messages.Push(MessageInfo.FromMetadataStream(3));

                        Assert.AreEqual(2, CurrentContext.Messages.Count);
                        Assert.AreEqual(3, CurrentContext.Messages.Current.Message);

                        // We await a certain amount of time just to make sure the await-construct does not destroy the context.
                        await Task.Delay(10);

                        Assert.AreEqual(2, CurrentContext.Messages.Count);
                        Assert.AreEqual(3, CurrentContext.Messages.Current.Message);
                    }

                    // As soon as the inner task has left its scope, it will see the outer scope again.
                    Assert.AreEqual(2, CurrentContext.Messages.Count);
                    Assert.AreEqual(2, CurrentContext.Messages.Current.Message);
                });
                
                // Here we will wait for the innerTask to create a new scope on its own thread.
                // Since the newly created inner scope should not be visible to this thread, the context
                // must have remained unchanged.
                waitHandleOuter.WaitOne();

                Assert.AreEqual(1, CurrentContext.Messages.Count);
                Assert.AreEqual(1, CurrentContext.Messages.Current.Message);                

                outerContext.Messages.Push(MessageInfo.FromOutputStream(2));

                Assert.AreEqual(2, CurrentContext.Messages.Count);
                Assert.AreEqual(2, CurrentContext.Messages.Current.Message);

                waitHandleInner.Set();

                // Finally, we await the inner task to complete before leaving our own scope. Again,
                // this should have no effect on our view of the context.
                await innerTask;

                Assert.AreEqual(2, CurrentContext.Messages.Count);
                Assert.AreEqual(2, CurrentContext.Messages.Current.Message);
            }
        }

        private static IMicroProcessorContext CurrentContext =>
            MicroProcessorContext.Current;

        private static MicroProcessorContextScope CreateScope(MicroProcessorContext context) =>
            MicroProcessorContext.CreateContextScope(context);
    }
}
