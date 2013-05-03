using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace YellowFlare.MessageProcessing
{
    [TestClass]
    public sealed class UnitOfWorkWrapperGroupTest
    {       
        [TestMethod]
        public void ResourceId_ReturnsResourceIdOfFirstItem()
        {
            var resourceItemA = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync());
            var group = new UnitOfWorkWrapperGroup(resourceItemA, resourceItemB);

            Assert.AreEqual("One", group.Group);
        }

        [TestMethod]
        public void ForceSynchronousFlush_ReturnsTrue_IfAnyItemForcesSynchronousFlush()
        {
            var resourceItemA = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneAsync());
            var group = new UnitOfWorkWrapperGroup(resourceItemA, resourceItemB);

            Assert.IsTrue(group.ForceSynchronousFlush);
        }

        [TestMethod]
        public void ForceSynchronousFlush_ReturnsFalse_IfNoItemsForceSynchronousFlush()
        {
            var resourceItemA = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneAsync());
            var resourceItemB = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneAsync());
            var group = new UnitOfWorkWrapperGroup(resourceItemA, resourceItemB);

            Assert.IsFalse(group.ForceSynchronousFlush);
        }

        [TestMethod]
        public void WrapsSameUnitOfWorkAs_ReturnsFalse_IfNoItemsWrapSameUnitOfWork()
        {
            var resourceItemA = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync());
            var resourceItemC = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync());
            var group = new UnitOfWorkWrapperGroup(resourceItemA, resourceItemB);

            Assert.IsFalse(group.WrapsSameUnitOfWorkAs(resourceItemC));
        }

        [TestMethod]
        public void WrapsSameUnitOfWorkAs_ReturnsTrue_IfAnyItemWrapsSameUnitOfWork()
        {
            var flushable = CreateUnitOfWorkOneSync();
            var resourceItemA = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkWrapperItem(flushable);
            var resourceItemC = new UnitOfWorkWrapperItem(flushable);
            var group = new UnitOfWorkWrapperGroup(resourceItemA, resourceItemB);

            Assert.IsTrue(group.WrapsSameUnitOfWorkAs(resourceItemC));
        }

        [TestMethod]
        public void TryMergeWith_ReturnsFalseAndNull_IfNewResourceWrapsSameUnitOfWork()
        {
            var flushable = CreateUnitOfWorkOneSync();
            var resourceItemA = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkWrapperItem(flushable);
            var resourceItemC = new UnitOfWorkWrapperItem(flushable);
            var group = new UnitOfWorkWrapperGroup(resourceItemA, resourceItemB);

            UnitOfWorkWrapperGroup mergedGroup;

            Assert.IsFalse(group.TryMergeWith(resourceItemC, out mergedGroup));
            Assert.IsNull(mergedGroup);
        }

        [TestMethod]
        public void TryMergeWith_ReturnsFalseAndNull_IfNewResourceHasResourceIdOfZero()
        {
            var resourceItemA = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync());
            var resourceItemC = new UnitOfWorkWrapperItem(CreateUnitOfWork());
            var group = new UnitOfWorkWrapperGroup(resourceItemA, resourceItemB);

            UnitOfWorkWrapperGroup mergedGroup;

            Assert.IsFalse(group.TryMergeWith(resourceItemC, out mergedGroup));
            Assert.IsNull(mergedGroup);
        }

        [TestMethod]
        public void TryMergeWith_ReturnsFalseAndNull_IfResourceIdentifiersAreNonZeroButNotEqual()
        {
            var resourceItemA = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync());
            var resourceItemC = new UnitOfWorkWrapperItem(CreateUnitOfWorkTwoSync());
            var group = new UnitOfWorkWrapperGroup(resourceItemA, resourceItemB);

            UnitOfWorkWrapperGroup mergedGroup;

            Assert.IsFalse(group.TryMergeWith(resourceItemC, out mergedGroup));
            Assert.IsNull(mergedGroup);
        }

        [TestMethod]
        public void TryMergeWith_ReturnsTrueAndResult_IfResourceIdentifiersAreNonZeroAndEqual()
        {
            var resourceItemA = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync());
            var resourceItemC = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync());
            var group = new UnitOfWorkWrapperGroup(resourceItemA, resourceItemB);

            UnitOfWorkWrapperGroup mergedGroup;

            Assert.IsTrue(group.TryMergeWith(resourceItemC, out mergedGroup));
            Assert.AreSame(group, mergedGroup);
        }

        [TestMethod]
        public void CollectResourcesThatRequireFlush_CollectsNoResources_IfNoItemsRequireFlush()
        {
            var flushableMockA = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMockA.Setup(flushable => flushable.RequiresFlush()).Returns(false);            

            var flushableMockB = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMockB.Setup(flushable => flushable.RequiresFlush()).Returns(false);

            var resourceItemA = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync(flushableMockA.Object));
            var resourceItemB = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync(flushableMockB.Object));
            var group = new UnitOfWorkWrapperGroup(resourceItemA, resourceItemB);

            var collectedResources = new List<UnitOfWorkWrapper>(2);
            group.CollectWrappersThatRequireFlush(collectedResources);
            Assert.AreEqual(0, collectedResources.Count);

            flushableMockA.Verify(flushable => flushable.RequiresFlush(), Times.Once());
            flushableMockB.Verify(flushable => flushable.RequiresFlush(), Times.Once());
        }

        [TestMethod]
        public void CollectResourcesThatRequireFlush_CollectsOneResourceItem_IfOneItemRequiresFlush()
        {
            var flushableMockA = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMockA.Setup(flushable => flushable.RequiresFlush()).Returns(false);

            var flushableMockB = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMockB.Setup(flushable => flushable.RequiresFlush()).Returns(true);

            var resourceItemA = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync(flushableMockA.Object));
            var resourceItemB = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync(flushableMockB.Object));
            var group = new UnitOfWorkWrapperGroup(resourceItemA, resourceItemB);

            var collectedResources = new List<UnitOfWorkWrapper>(1);
            group.CollectWrappersThatRequireFlush(collectedResources);

            Assert.AreEqual(1, collectedResources.Count);
            Assert.IsInstanceOfType(collectedResources[0], typeof(UnitOfWorkWrapperItem));

            flushableMockA.Verify(flushable => flushable.RequiresFlush(), Times.Once());
            flushableMockB.Verify(flushable => flushable.RequiresFlush(), Times.Once());
        }

        [TestMethod]
        public void CollectResourcesThatRequireFlush_CollectsResourceGroup_IfManyItemsRequiresFlush()
        {
            var flushableMockA = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMockA.Setup(flushable => flushable.RequiresFlush()).Returns(true);

            var flushableMockB = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMockB.Setup(flushable => flushable.RequiresFlush()).Returns(false);

            var flushableMockC = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMockC.Setup(flushable => flushable.RequiresFlush()).Returns(true);
            
            var group = new UnitOfWorkWrapperGroup(new []
            {
                 new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync(flushableMockA.Object)),
                 new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync(flushableMockB.Object)),
                 new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync(flushableMockC.Object))
            });

            var collectedResources = new List<UnitOfWorkWrapper>(1);
            group.CollectWrappersThatRequireFlush(collectedResources);

            Assert.AreEqual(1, collectedResources.Count);
            Assert.IsInstanceOfType(collectedResources[0], typeof(UnitOfWorkWrapperGroup));
            Assert.AreNotSame(group, collectedResources[0]);

            flushableMockA.Verify(flushable => flushable.RequiresFlush(), Times.Once());
            flushableMockB.Verify(flushable => flushable.RequiresFlush(), Times.Once());
        }

        [TestMethod]
        public void CollectResourcesThatRequireFlush_CollectsSelf_IfAllItemsRequiresFlush()
        {
            var flushableMockA = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMockA.Setup(flushable => flushable.RequiresFlush()).Returns(true);

            var flushableMockB = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMockB.Setup(flushable => flushable.RequiresFlush()).Returns(true);

            var flushableMockC = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMockC.Setup(flushable => flushable.RequiresFlush()).Returns(true);

            var group = new UnitOfWorkWrapperGroup(new[]
            {
                 new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync(flushableMockA.Object)),
                 new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync(flushableMockB.Object)),
                 new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync(flushableMockC.Object))
            });

            var collectedResources = new List<UnitOfWorkWrapper>(1);
            group.CollectWrappersThatRequireFlush(collectedResources);

            Assert.AreEqual(1, collectedResources.Count);            
            Assert.AreSame(group, collectedResources[0]);

            flushableMockA.Verify(flushable => flushable.RequiresFlush(), Times.Once());
            flushableMockB.Verify(flushable => flushable.RequiresFlush(), Times.Once());
        }

        [TestMethod]
        public void Flush_FlushesAllResources()
        {
            var flushableMockA = new Mock<IUnitOfWork>(MockBehavior.Strict);
            flushableMockA.Setup(flushable => flushable.Flush());

            var flushableMockB = new Mock<IUnitOfWork>(MockBehavior.Strict);            
            flushableMockB.Setup(flushable => flushable.Flush());

            var resourceItemA = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync(flushableMockA.Object));
            var resourceItemB = new UnitOfWorkWrapperItem(CreateUnitOfWorkOneSync(flushableMockB.Object));
            var group = new UnitOfWorkWrapperGroup(resourceItemA, resourceItemB);

            group.Flush();

            flushableMockA.Verify(flushable => flushable.Flush(), Times.Once());           
            flushableMockB.Verify(flushable => flushable.Flush(), Times.Once());
        }

        private static IUnitOfWork CreateUnitOfWorkOneSync(IUnitOfWork flushable = null)
        {
            return new UnitOfWorkWithAttributeOneSync(flushable ?? CreateUnitOfWork());
        }

        private static IUnitOfWork CreateUnitOfWorkOneAsync(IUnitOfWork flushable = null)
        {
            return new UnitOfWorkWithAttributeOneAsync(flushable ?? CreateUnitOfWork());
        }

        private static IUnitOfWork CreateUnitOfWorkTwoSync(IUnitOfWork flushable = null)
        {
            return new UnitOfWorkWithAttributeTwoSync(flushable ?? CreateUnitOfWork());
        }

        private static IUnitOfWork CreateUnitOfWork()
        {
            return new Mock<IUnitOfWork>().Object;
        }
    }
}
