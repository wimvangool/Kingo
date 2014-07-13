using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Timer = System.Timers.Timer;

namespace YellowFlare.MessageProcessing.Requests
{
    [TestClass]
    public sealed class MemoryCacheTest
    {
        private MemoryCache _cache;

        [TestInitialize]
        public void Setup()
        {
            _cache = new MemoryCache();
        }

        [TestMethod]
        public void GetOrAdd_AddsNewValue_IfKeyDoesNotAlreadyExist()
        {
            var key = new object();
            var value = new object();
            var cacheValue = _cache.GetOrAdd(key, keyAlias => new QueryCacheValue(value));

            Assert.IsNotNull(cacheValue);
            Assert.AreSame(value, cacheValue.Access<object>());
            Assert.IsTrue(cacheValue.Lifetime.HasStarted);
        }

        [TestMethod]
        public void GetOrAdd_RetrievesValue_IfKeyAlreadyExists()
        {
            var key = new object();
            var value = new object();
            var cacheValueA = _cache.GetOrAdd(key, keyAlias => new QueryCacheValue(value));
            var cacheValueB = _cache.GetOrAdd(key, keyAlias => null);

            Assert.IsNotNull(cacheValueA);
            Assert.AreSame(cacheValueA, cacheValueB);            
        }

        [TestMethod]
        public void TryGet_ReturnsFalse_IfKeyDoesNotAlreadyExist()
        {
            QueryCacheValue value;

            Assert.IsFalse(_cache.TryGetValue(new object(), out value));
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TryGet_ReturnsTrue_IfKeyAlreadyExists()
        {
            var key = new object();
            var value = new object();
            var cacheValue = _cache.GetOrAdd(key, keyAlias => new QueryCacheValue(value));

            QueryCacheValue retrievedValue;

            Assert.IsTrue(_cache.TryGetValue(key, out retrievedValue));
            Assert.AreSame(cacheValue, retrievedValue);
        }

        [TestMethod]
        public void TryGet_ReturnsFalse_IfKeyWasRemovedManually()
        {
            var key = new object();
            var value = new object();
            var cacheValue = _cache.GetOrAdd(key, keyAlias => new QueryCacheValue(value));

            QueryCacheValue retrievedValue;

            Assert.IsTrue(_cache.TryRemove(key, out retrievedValue));
            Assert.AreSame(cacheValue, retrievedValue);
            
            Assert.IsFalse(_cache.TryGetValue(key, out retrievedValue));
            Assert.IsNull(retrievedValue);
        }

        [TestMethod]
        public void TryGet_ReturnsFalse_IfKeyWasRemovedAutomatically()
        {
            using (var waitHandle = new ManualResetEventSlim())
            using (var timer = new Timer(100))
            {
                // As soon as the cacheValue expires, this timer is started.
                // Then, after 100 milliseconds, the wait-handle will be set,
                // after which it is checked whether or not the value
                // was actually removed from the cache.
                timer.Elapsed += (s, e) => waitHandle.Set();
            
                var key = new object();
                var value = new object();
                var cacheValue = _cache.GetOrAdd(key, keyAlias =>
                {
                    var lifetime = new TimerBasedLifetime(TimeSpan.FromMilliseconds(10));
                    lifetime.Expired += (s, e) => timer.Start();
                    return new QueryCacheValue(value, lifetime);
                });
                Assert.AreSame(value, cacheValue.Access<object>());

                if (waitHandle.Wait(TimeSpan.FromMilliseconds(500)))
                {
                    QueryCacheValue retrievedValue;

                    Assert.IsFalse(_cache.TryGetValue(key, out retrievedValue));
                    Assert.IsNull(retrievedValue);
                    return;
                }
                TimerBasedLifetimeTest.FailBecauseLifetimeDidNotExpire();                
            }
        }
    }
}
