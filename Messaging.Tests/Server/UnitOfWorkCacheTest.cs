using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Messaging.Server
{
    [TestClass]
    public sealed class UnitOfWorkCacheTest
    {
        #region [====== Setup and Teardown ======]

        private ScopeSpecificCache _cache;

        [TestInitialize]
        public void Setup()
        {
            _cache = new ScopeSpecificCache();
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
            IScopeSpecificCacheEntry<int> cachedItem = _cache.Add(10);
            int value;

            Assert.IsNotNull(cachedItem);
            Assert.IsTrue(cachedItem.TryGetValue(out value));
            Assert.AreEqual(10, value);
        }

        [TestMethod]
        public void Dispose_InvalidatesReturnedItem()
        {
            IScopeSpecificCacheEntry<int> cachedItem = _cache.Add(10);
            int value;

            _cache.Dispose();

            Assert.IsNotNull(cachedItem);
            Assert.IsFalse(cachedItem.TryGetValue(out value));
            Assert.AreEqual(0, value);
        }

        #endregion        
    }
}
