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
            var resourceItemA = new UnitOfWorkItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkItem(CreateUnitOfWorkOneSync());
            var group = new UnitOfWorkGroup(resourceItemA, resourceItemB);

            Assert.AreEqual("One", group.FlushGroup);
        }

        [TestMethod]
        public void CanBeFlushedAsynchronously_ReturnsFalse_IfAnyItemForcesSynchronousFlush()
        {
            var resourceItemA = new UnitOfWorkItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkItem(CreateUnitOfWorkOneAsync());
            var group = new UnitOfWorkGroup(resourceItemA, resourceItemB);

            Assert.IsFalse(group.CanBeFlushedAsynchronously);
        }

        [TestMethod]
        public void CaneBeFlushedAsynchronously_ReturnsTrue_IfAllItemsCanbeFlushedAsynchrously()
        {
            var resourceItemA = new UnitOfWorkItem(CreateUnitOfWorkOneAsync());
            var resourceItemB = new UnitOfWorkItem(CreateUnitOfWorkOneAsync());
            var group = new UnitOfWorkGroup(resourceItemA, resourceItemB);

            Assert.IsTrue(group.CanBeFlushedAsynchronously);
        }

        [TestMethod]
        public void WrapsSameUnitOfWorkAs_ReturnsFalse_IfNoItemsWrapSameUnitOfWork()
        {
            var resourceItemA = new UnitOfWorkItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkItem(CreateUnitOfWorkOneSync());
            var resourceItemC = new UnitOfWorkItem(CreateUnitOfWorkOneSync());
            var group = new UnitOfWorkGroup(resourceItemA, resourceItemB);

            Assert.IsFalse(group.WrapsSameUnitOfWorkAs(resourceItemC));
        }

        [TestMethod]
        public void WrapsSameUnitOfWorkAs_ReturnsTrue_IfAnyItemWrapsSameUnitOfWork()
        {
            var flushable = CreateUnitOfWorkOneSync();
            var resourceItemA = new UnitOfWorkItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkItem(flushable);
            var resourceItemC = new UnitOfWorkItem(flushable);
            var group = new UnitOfWorkGroup(resourceItemA, resourceItemB);

            Assert.IsTrue(group.WrapsSameUnitOfWorkAs(resourceItemC));
        }

        [TestMethod]
        public void TryMergeWith_ReturnsFalseAndNull_IfNewResourceWrapsSameUnitOfWork()
        {
            var flushable = CreateUnitOfWorkOneSync();
            var resourceItemA = new UnitOfWorkItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkItem(flushable);
            var resourceItemC = new UnitOfWorkItem(flushable);
            var group = new UnitOfWorkGroup(resourceItemA, resourceItemB);

            UnitOfWorkGroup mergedGroup;

            Assert.IsFalse(group.TryMergeWith(resourceItemC, out mergedGroup));
            Assert.IsNull(mergedGroup);
        }

        [TestMethod]
        public void TryMergeWith_ReturnsFalseAndNull_IfNewResourceHasResourceIdOfZero()
        {
            var resourceItemA = new UnitOfWorkItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkItem(CreateUnitOfWorkOneSync());
            var resourceItemC = new UnitOfWorkItem(CreateUnitOfWork());
            var group = new UnitOfWorkGroup(resourceItemA, resourceItemB);

            UnitOfWorkGroup mergedGroup;

            Assert.IsFalse(group.TryMergeWith(resourceItemC, out mergedGroup));
            Assert.IsNull(mergedGroup);
        }

        [TestMethod]
        public void TryMergeWith_ReturnsFalseAndNull_IfResourceIdentifiersAreNonZeroButNotEqual()
        {
            var resourceItemA = new UnitOfWorkItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkItem(CreateUnitOfWorkOneSync());
            var resourceItemC = new UnitOfWorkItem(CreateUnitOfWorkTwoSync());
            var group = new UnitOfWorkGroup(resourceItemA, resourceItemB);

            UnitOfWorkGroup mergedGroup;

            Assert.IsFalse(group.TryMergeWith(resourceItemC, out mergedGroup));
            Assert.IsNull(mergedGroup);
        }

        [TestMethod]
        public void TryMergeWith_ReturnsTrueAndResult_IfResourceIdentifiersAreNonZeroAndEqual()
        {
            var resourceItemA = new UnitOfWorkItem(CreateUnitOfWorkOneSync());
            var resourceItemB = new UnitOfWorkItem(CreateUnitOfWorkOneSync());
            var resourceItemC = new UnitOfWorkItem(CreateUnitOfWorkOneSync());
            var group = new UnitOfWorkGroup(resourceItemA, resourceItemB);

            UnitOfWorkGroup mergedGroup;

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

            var resourceItemA = new UnitOfWorkItem(CreateUnitOfWorkOneSync(flushableMockA.Object));
            var resourceItemB = new UnitOfWorkItem(CreateUnitOfWorkOneSync(flushableMockB.Object));
            var group = new UnitOfWorkGroup(resourceItemA, resourceItemB);

            var collectedResources = new List<UnitOfWorkWrapper>(2);
            group.CollectUnitsThatRequireFlush(collectedResources);
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

            var resourceItemA = new UnitOfWorkItem(CreateUnitOfWorkOneSync(flushableMockA.Object));
            var resourceItemB = new UnitOfWorkItem(CreateUnitOfWorkOneSync(flushableMockB.Object));
            var group = new UnitOfWorkGroup(resourceItemA, resourceItemB);

            var collectedResources = new List<UnitOfWorkWrapper>(1);
            group.CollectUnitsThatRequireFlush(collectedResources);

            Assert.AreEqual(1, collectedResources.Count);
            Assert.IsInstanceOfType(collectedResources[0], typeof(UnitOfWorkItem));

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
            
            var group = new UnitOfWorkGroup(new []
            {
                 new UnitOfWorkItem(CreateUnitOfWorkOneSync(flushableMockA.Object)),
                 new UnitOfWorkItem(CreateUnitOfWorkOneSync(flushableMockB.Object)),
                 new UnitOfWorkItem(CreateUnitOfWorkOneSync(flushableMockC.Object))
            });

            var collectedResources = new List<UnitOfWorkWrapper>(1);
            group.CollectUnitsThatRequireFlush(collectedResources);

            Assert.AreEqual(1, collectedResources.Count);
            Assert.IsInstanceOfType(collectedResources[0], typeof(UnitOfWorkGroup));
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

            var group = new UnitOfWorkGroup(new[]
            {
                 new UnitOfWorkItem(CreateUnitOfWorkOneSync(flushableMockA.Object)),
                 new UnitOfWorkItem(CreateUnitOfWorkOneSync(flushableMockB.Object)),
                 new UnitOfWorkItem(CreateUnitOfWorkOneSync(flushableMockC.Object))
            });

            var collectedResources = new List<UnitOfWorkWrapper>(1);
            group.CollectUnitsThatRequireFlush(collectedResources);

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

            var resourceItemA = new UnitOfWorkItem(CreateUnitOfWorkOneSync(flushableMockA.Object));
            var resourceItemB = new UnitOfWorkItem(CreateUnitOfWorkOneSync(flushableMockB.Object));
            var group = new UnitOfWorkGroup(resourceItemA, resourceItemB);

            group.Flush();

            flushableMockA.Verify(flushable => flushable.Flush(), Times.Once());           
            flushableMockB.Verify(flushable => flushable.Flush(), Times.Once());
        }

        private static IUnitOfWork CreateUnitOfWorkOneSync(IUnitOfWork flushable = null)
        {
            return new UnitOfWorkOneSyncTest(flushable ?? CreateUnitOfWork());
        }

        private static IUnitOfWork CreateUnitOfWorkOneAsync(IUnitOfWork flushable = null)
        {
            return new UnitOfWorkOneAsyncTest(flushable ?? CreateUnitOfWork());
        }

        private static IUnitOfWork CreateUnitOfWorkTwoSync(IUnitOfWork flushable = null)
        {
            return new UnitOfWorkTwoSyncTest(flushable ?? CreateUnitOfWork());
        }

        private static IUnitOfWork CreateUnitOfWork()
        {
            return new Mock<IUnitOfWork>().Object;
        }
    }
}
