using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace YellowFlare.MessageProcessing
{
    [TestClass]
    public sealed class UnitOfWorkWrapperItemTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfUnitOfWorkIsNull()
        {
            new UnitOfWorkItem(null);
        }                           

        [TestMethod]
        public void WrapsSameUnitOfWorkAs_ReturnsFalse_IfResourceDoesNotWrapSameUnitOfWork()
        {
            var resourceA = new UnitOfWorkItem(CreateUnitOfWork());
            var resourceB = new UnitOfWorkItem(CreateUnitOfWork());

            Assert.IsFalse(resourceA.WrapsSameUnitOfWorkAs(resourceB));
            Assert.IsFalse(resourceB.WrapsSameUnitOfWorkAs(resourceA));
        }

        [TestMethod]
        public void WrapsSameUnitOfWorkAs_ReturnsTrue_IfResourceDoesWrapSameUnitOfWork()
        {
            var flushable = CreateUnitOfWork();
            var resourceA = new UnitOfWorkItem(flushable);
            var resourceB = new UnitOfWorkItem(flushable);

            Assert.IsTrue(resourceA.WrapsSameUnitOfWorkAs(resourceB));
            Assert.IsTrue(resourceB.WrapsSameUnitOfWorkAs(resourceA));
        }

        [TestMethod]
        public void TryMergeWith_ReturnsFalseAndNull_IfNewResourceWrapsSameUnitOfWork()
        {
            var flushable = CreateUnitOfWorkWithAttributeOneSync();
            var resourceA = new UnitOfWorkItem(flushable);
            var resourceB = new UnitOfWorkItem(flushable);

            UnitOfWorkGroup group;

            Assert.IsFalse(resourceA.TryMergeWith(resourceB, out group));
            Assert.IsNull(group);
        }

        [TestMethod]
        public void TryMergeWith_ReturnsFalseAndNull_IfNewResourceHasResourceIdOfZero()
        {            
            var resourceA = new UnitOfWorkItem(CreateUnitOfWork());
            var resourceB = new UnitOfWorkItem(CreateUnitOfWork());

            UnitOfWorkGroup group;

            Assert.IsFalse(resourceA.TryMergeWith(resourceB, out group));
            Assert.IsNull(group);
        }

        [TestMethod]
        public void TryMergeWith_ReturnsFalseAndNull_IfResourceIdentifiersAreNonZeroButNotEqual()
        {
            var resourceA = new UnitOfWorkItem(CreateUnitOfWorkWithAttributeOneSync());
            var resourceB = new UnitOfWorkItem(CreateUnitOfWorkWithAttributeTwoSync());

            UnitOfWorkGroup group;

            Assert.IsFalse(resourceA.TryMergeWith(resourceB, out group));
            Assert.IsNull(group);
        }

        [TestMethod]
        public void TryMergeWith_ReturnsTrueAndResult_IfResourceIdentifiersAreNonZeroAndEqual()
        {
            var resourceA = new UnitOfWorkItem(CreateUnitOfWorkWithAttributeOneSync());
            var resourceB = new UnitOfWorkItem(CreateUnitOfWorkWithAttributeOneSync());

            UnitOfWorkGroup group;

            Assert.IsTrue(resourceA.TryMergeWith(resourceB, out group));
            Assert.IsNotNull(group);
        }

        [TestMethod]
        public void RequiresFlush_ReturnsFalse_IfUnitOfWorkReturnsFalse()
        {
            var flushableMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMock.Setup(flushable => flushable.RequiresFlush()).Returns(false);
            var resource = new UnitOfWorkItem(flushableMock.Object);

            Assert.IsFalse(resource.RequiresFlush());

            flushableMock.Verify(flushable => flushable.RequiresFlush(), Times.Once());
        }

        [TestMethod]
        public void RequiresFlush_ReturnsTrue_IfUnitOfWorkReturnsTrue()
        {
            var flushableMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMock.Setup(flushable => flushable.RequiresFlush()).Returns(true);
            var resource = new UnitOfWorkItem(flushableMock.Object);

            Assert.IsTrue(resource.RequiresFlush());

            flushableMock.Verify(flushable => flushable.RequiresFlush(), Times.Once());
        }

        [TestMethod]
        public void Flush_FlushesWrappedUnitOfWork()
        {
            var flushableMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMock.Setup(flushable => flushable.Flush());
            var resource = new UnitOfWorkItem(flushableMock.Object);

            resource.Flush();

            flushableMock.Verify(flushable => flushable.Flush(), Times.Once());
        }

        private static IUnitOfWork CreateUnitOfWork()
        {
            return new Mock<IUnitOfWork>().Object;
        }

        private static IUnitOfWork CreateUnitOfWorkWithAttributeOneSync()
        {
            return new UnitOfWorkWithAttributeOneSync(CreateUnitOfWork());
        }

        private static IUnitOfWork CreateUnitOfWorkWithAttributeTwoSync()
        {
            return new UnitOfWorkWithAttributeTwoSync(CreateUnitOfWork());
        }
    }
}
