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
            new UnitOfWorkWrapperItem(null);
        }

        [TestMethod]
        public void ResourceId_ReturnsZero_IfNoFlushHintAttributeWasSpecified()
        {
            var resource = new UnitOfWorkWrapperItem(CreateUnitOfWork());

            Assert.AreEqual(null, resource.Group);
        }

        [TestMethod]
        public void ForceSynchronousFlush_ReturnsFalse_IfNoFlushHintAttributeWasSpecified()
        {
            var resource = new UnitOfWorkWrapperItem(CreateUnitOfWork());

            Assert.IsFalse(resource.ForceSynchronousFlush);
        }

        [TestMethod]
        public void ResourceId_ReturnsValueOfAttribute_IfFlushHintAttributeWasSpecified()
        {
            var resource = new UnitOfWorkWrapperItem(CreateUnitOfWorkWithAttributeOneSync());

            Assert.AreEqual("One", resource.Group);
        }

        [TestMethod]
        public void ForceSynchronousFlush_ReturnsTrue_IfFlushHintAttributeSpecifiesTrue()
        {
            var resource = new UnitOfWorkWrapperItem(CreateUnitOfWorkWithAttributeOneSync());

            Assert.IsTrue(resource.ForceSynchronousFlush);
        }

        [TestMethod]
        public void WrapsSameUnitOfWorkAs_ReturnsFalse_IfResourceDoesNotWrapSameUnitOfWork()
        {
            var resourceA = new UnitOfWorkWrapperItem(CreateUnitOfWork());
            var resourceB = new UnitOfWorkWrapperItem(CreateUnitOfWork());

            Assert.IsFalse(resourceA.WrapsSameUnitOfWorkAs(resourceB));
            Assert.IsFalse(resourceB.WrapsSameUnitOfWorkAs(resourceA));
        }

        [TestMethod]
        public void WrapsSameUnitOfWorkAs_ReturnsTrue_IfResourceDoesWrapSameUnitOfWork()
        {
            var flushable = CreateUnitOfWork();
            var resourceA = new UnitOfWorkWrapperItem(flushable);
            var resourceB = new UnitOfWorkWrapperItem(flushable);

            Assert.IsTrue(resourceA.WrapsSameUnitOfWorkAs(resourceB));
            Assert.IsTrue(resourceB.WrapsSameUnitOfWorkAs(resourceA));
        }

        [TestMethod]
        public void TryMergeWith_ReturnsFalseAndNull_IfNewResourceWrapsSameUnitOfWork()
        {
            var flushable = CreateUnitOfWorkWithAttributeOneSync();
            var resourceA = new UnitOfWorkWrapperItem(flushable);
            var resourceB = new UnitOfWorkWrapperItem(flushable);

            UnitOfWorkWrapperGroup group;

            Assert.IsFalse(resourceA.TryMergeWith(resourceB, out group));
            Assert.IsNull(group);
        }

        [TestMethod]
        public void TryMergeWith_ReturnsFalseAndNull_IfNewResourceHasResourceIdOfZero()
        {            
            var resourceA = new UnitOfWorkWrapperItem(CreateUnitOfWork());
            var resourceB = new UnitOfWorkWrapperItem(CreateUnitOfWork());

            UnitOfWorkWrapperGroup group;

            Assert.IsFalse(resourceA.TryMergeWith(resourceB, out group));
            Assert.IsNull(group);
        }

        [TestMethod]
        public void TryMergeWith_ReturnsFalseAndNull_IfResourceIdentifiersAreNonZeroButNotEqual()
        {
            var resourceA = new UnitOfWorkWrapperItem(CreateUnitOfWorkWithAttributeOneSync());
            var resourceB = new UnitOfWorkWrapperItem(CreateUnitOfWorkWithAttributeTwoSync());

            UnitOfWorkWrapperGroup group;

            Assert.IsFalse(resourceA.TryMergeWith(resourceB, out group));
            Assert.IsNull(group);
        }

        [TestMethod]
        public void TryMergeWith_ReturnsTrueAndResult_IfResourceIdentifiersAreNonZeroAndEqual()
        {
            var resourceA = new UnitOfWorkWrapperItem(CreateUnitOfWorkWithAttributeOneSync());
            var resourceB = new UnitOfWorkWrapperItem(CreateUnitOfWorkWithAttributeOneSync());

            UnitOfWorkWrapperGroup group;

            Assert.IsTrue(resourceA.TryMergeWith(resourceB, out group));
            Assert.IsNotNull(group);
        }

        [TestMethod]
        public void RequiresFlush_ReturnsFalse_IfUnitOfWorkReturnsFalse()
        {
            var flushableMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMock.Setup(flushable => flushable.RequiresFlush()).Returns(false);
            var resource = new UnitOfWorkWrapperItem(flushableMock.Object);

            Assert.IsFalse(resource.RequiresFlush());

            flushableMock.Verify(flushable => flushable.RequiresFlush(), Times.Once());
        }

        [TestMethod]
        public void RequiresFlush_ReturnsTrue_IfUnitOfWorkReturnsTrue()
        {
            var flushableMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMock.Setup(flushable => flushable.RequiresFlush()).Returns(true);
            var resource = new UnitOfWorkWrapperItem(flushableMock.Object);

            Assert.IsTrue(resource.RequiresFlush());

            flushableMock.Verify(flushable => flushable.RequiresFlush(), Times.Once());
        }

        [TestMethod]
        public void Flush_FlushesWrapdUnitOfWork()
        {
            var flushableMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMock.Setup(flushable => flushable.Flush());
            var resource = new UnitOfWorkWrapperItem(flushableMock.Object);

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
