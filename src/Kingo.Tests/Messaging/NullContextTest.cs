using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class NullContextTest
    {        
        [TestMethod]
        public void Current_IsNotNull_IfNoScopeHasBeenCreated()
        {
            Assert.IsNotNull(Context);
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue()
        {
            Assert.AreEqual(string.Empty, Context.ToString());
        }

        [TestMethod]
        public void MessageStack_IsEmpty_IfContextIsNone()
        {                        
            Assert.IsNotNull(Context.StackTrace);
            Assert.AreEqual(0, Context.StackTrace.Count);
            Assert.IsNull(Context.StackTrace.CurrentOperation);
            Assert.IsFalse(Context.StackTrace.Any());
            Assert.AreEqual(string.Empty, Context.StackTrace.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void MessageStackIndexer_Throws_ForAnyValue()
        {
            Context.StackTrace[0].IgnoreValue();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EventStream_Publish_Throws_ForAnyItem()
        {
            Context.EventBus.Publish(new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Cache_GetItem_Throws_IfKeyIsNull()
        {
            Context.UnitOfWork.Cache[null].IgnoreValue();
        }

        [TestMethod]        
        public void Cache_GetItem_ReturnsNull_ForAnyKey()
        {
            Assert.IsNull(Context.UnitOfWork.Cache[Guid.NewGuid().ToString()]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Cache_SetItem_Throws_ForAnyKey()
        {
            Context.UnitOfWork.Cache[Guid.NewGuid().ToString()] = new object();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Cache_Remove_Throws_ForAnyKey()
        {
            Context.UnitOfWork.Cache.Remove(Guid.NewGuid().ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EnlistAsync_Throws_IfUnitOfWorkIsNull()
        {
            Context.UnitOfWork.EnlistAsync(null);
        }

        [TestMethod]
        public async Task EnlistAsync_DoesNothing_IfUnitOfWorkIsNotNull_And_DoesNotRequireFlush()
        {
            var unitOfWork = new UnitOfWorkSpy(false);

            await Context.UnitOfWork.EnlistAsync(unitOfWork);

            unitOfWork.AssertRequiresFlushCountIs(1);
            unitOfWork.AssertFlushCountIs(0);
        }

        [TestMethod]
        public async Task EnlistAsync_ImmediatelyFlushesUnitOfWork_IfUnitOfWorkIsNotNull_And_RequiresFlush()
        {
            var unitOfWork = new UnitOfWorkSpy(true);

            await Context.UnitOfWork.EnlistAsync(unitOfWork);

            unitOfWork.AssertRequiresFlushCountIs(1);
            unitOfWork.AssertFlushCountIs(1);
        }             

        private static IMicroProcessorContext Context =>
            MicroProcessorContext.Current;
    }
}
