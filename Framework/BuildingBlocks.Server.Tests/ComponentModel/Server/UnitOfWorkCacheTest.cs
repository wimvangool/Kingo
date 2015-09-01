using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.ComponentModel.Server
{
    [TestClass]
    public sealed class UnitOfWorkCacheTest
    {
        #region [====== Setup and Teardown ======]

        private DependencyCache _cache;

        [TestInitialize]
        public void Setup()
        {
            _cache = new DependencyCache();
        }

        [TestCleanup]
        public void Teardown()
        {
            _cache.Dispose();
        }

        #endregion

        #region [====== Tests ======]

        [TestMethod]
        public void Add_AddsTheValueAndReturnsValidItem()
        {
            IDependencyCacheEntry<int> cachedItem = _cache.Add(10);
            int value;

            Assert.IsNotNull(cachedItem);
            Assert.IsTrue(cachedItem.TryGetValue(out value));
            Assert.AreEqual(10, value);
        }

        [TestMethod]
        public void Dispose_InvalidatesReturnedItem()
        {
            IDependencyCacheEntry<int> cachedItem = _cache.Add(10);
            int value;

            _cache.Dispose();

            Assert.IsNotNull(cachedItem);
            Assert.IsFalse(cachedItem.TryGetValue(out value));
            Assert.AreEqual(0, value);
        }

        #endregion        
    }
}
