using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace YellowFlare.MessageProcessing
{
    [TestClass]
    public class MessageProcessorContextCacheTest
    {
        #region [====== Setup and Teardown ======]

        private MessageProcessorContextCache _cache;

        [TestInitialize]
        public void Setup()
        {
            _cache = new MessageProcessorContextCache();
        }

        [TestCleanup]
        public void Teardown()
        {
            _cache.Dispose();
        }

        #endregion

        #region [====== Tests - Properties and Indexers  ======]

        [TestMethod]        
        public void Count_ReturnsNumberOfItemsInCache()
        {
            Assert.AreEqual(0, _cache.Count);
        }

        [TestMethod]
        public void IsReadOnly_ReturnsFalse()
        {
            Assert.IsFalse(_cache.IsReadOnly);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetItem_Throws_IfCacheDoesNotContainKey()
        {
            var value = _cache[Guid.NewGuid()];
        }

        [TestMethod]
        public void GetItem_ReturnsValue_IfCacheContainsKey()
        {
            var key = Guid.NewGuid();
            var value = new object();

            _cache.Add(key, value);

            Assert.AreSame(value, _cache[key]);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SetItem_Throws_IfCacheIsDisposed()
        {
            _cache.Dispose();
            _cache[Guid.NewGuid()] = new object();
        }

        [TestMethod]
        public void SetItem_SetsTheValue_IfCacheDoesNotContainKeyYet()
        {
            var key = Guid.NewGuid();
            var value = new object();

            _cache[key] = value;

            Assert.AreSame(value, _cache[key]);
        }

        [TestMethod]
        public void SetItem_ReplacesExistingValue_IfCacheAlreadyContainsSomeValueWithSameKey()
        {
            var key = Guid.NewGuid();
            var value = new object();

            _cache[key] = new object();
            _cache[key] = value;

            Assert.AreSame(value, _cache[key]);
        }

        [TestMethod]
        public void Keys_ReturnsCollectionWithAllKeys()
        {
            var keyA = Guid.NewGuid();
            var keyB = Guid.NewGuid();

            _cache[keyA] = new object();
            _cache[keyB] = new object();

            var keyCollection = _cache.Keys;

            Assert.AreEqual(2, keyCollection.Count);
            Assert.IsTrue(keyCollection.Contains(keyA));
            Assert.IsTrue(keyCollection.Contains(keyB));
        }

        [TestMethod]
        public void Values_ReturnsCollectionWithAllValues()
        {
            var valueA = new object();
            var valueB = new object();

            _cache[Guid.NewGuid()] = valueA;
            _cache[Guid.NewGuid()] = valueB;

            var valueCollection = _cache.Values;

            Assert.AreEqual(2, valueCollection.Count);
            Assert.IsTrue(valueCollection.Contains(valueA));
            Assert.IsTrue(valueCollection.Contains(valueB));
        }

        #endregion

        #region [====== Tests - Write Methods ======]

        [TestMethod]
        public void Dispose_DisposesCacheAndAllDisposableValues()
        {
            Mock<IDisposable> resourceMock = new Mock<IDisposable>(MockBehavior.Strict);
            resourceMock.Setup(resource => resource.Dispose());

            _cache.Add(Guid.NewGuid(), resourceMock.Object);
            _cache.Dispose();

            resourceMock.VerifyAll();
        }

        [TestMethod]
        public void Dispose_DoesNotThrow_IfCacheContainsNullValues()
        {
            _cache.Add(Guid.NewGuid(), null);
            _cache.Dispose();
        }

        [TestMethod]
        public void Dispose_DoesNotThrow_IfCalledTwice()
        {
            _cache.Dispose();
            _cache.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Add_Throws_IfCacheIsDisposed()
        {
            _cache.Dispose();
            _cache.Add(Guid.NewGuid(), new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_Throws_IfKeyAlreadyExists()
        {
            var key = Guid.NewGuid();

            _cache.Add(key, new object());
            _cache.Add(key, new object());
        }

        [TestMethod]
        public void Add_AddsTheNewValue_IfItDoesNotThrow()
        {
            _cache.Add(Guid.NewGuid(), new object());

            Assert.AreEqual(1, _cache.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void AddKeyValuePair_Throws_IfCacheIsDisposed()
        {
            _cache.Dispose();
            _cache.Add(new KeyValuePair<Guid, object>(Guid.NewGuid(), new object()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddKeyValuePair_Throws_IfKeyAlreadyExists()
        {
            var key = Guid.NewGuid();

            _cache.Add(new KeyValuePair<Guid, object>(key, new object()));
            _cache.Add(new KeyValuePair<Guid, object>(key, new object()));
        }

        [TestMethod]
        public void AddKeyValuePair_AddsTheNewValue_IfItDoesNotThrow()
        {
            _cache.Add(new KeyValuePair<Guid, object>(Guid.NewGuid(), new object()));

            Assert.AreEqual(1, _cache.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Remove_Throws_IfCacheIsDisposed()
        {
            _cache.Dispose();
            _cache.Remove(Guid.NewGuid());
        }

        [TestMethod]
        public void Remove_ReturnsFalse_IfCacheDoesNotContainKey()
        {
           Assert.IsFalse(_cache.Remove(Guid.NewGuid())); 
        }

        [TestMethod]
        public void Remove_ReturnsTrue_IfCacheDoesContainKey()
        {
            var key = Guid.NewGuid();

            _cache.Add(key, new object());

            Assert.IsTrue(_cache.Remove(key));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void RemoveKeyValuePair_Throws_IfCacheIsDisposed()
        {
            _cache.Dispose();
            _cache.Remove(new KeyValuePair<Guid, object>(Guid.NewGuid(), null));
        }

        [TestMethod]
        public void RemoveKeyValuePair_ReturnsFalse_IfCacheDoesNotContainKey()
        {
            Assert.IsFalse(_cache.Remove(new KeyValuePair<Guid, object>(Guid.NewGuid(), null)));
        }

        [TestMethod]
        public void RemoveKeyValuePair_ReturnsTrue_IfCacheDoesContainKeyValuePair()
        {
            var key = Guid.NewGuid();

            _cache.Add(key, null);

            Assert.IsTrue(_cache.Remove(new KeyValuePair<Guid, object>(key, null)));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Clear_Throws_IfCacheIsDisposed()
        {
            _cache.Dispose();
            _cache.Clear();
        }

        [TestMethod]
        public void Clear_ClearsTheCache_IfItDoesNotThrow()
        {
            _cache.Add(Guid.NewGuid(), null);
            _cache.Clear();

            Assert.AreEqual(0, _cache.Count);
        }

        #endregion

        #region [====== Tests - Read Methods ======]

        [TestMethod]
        public void ContainsKey_ReturnsFalse_IfCacheDoesNotContainKey()
        {
            Assert.IsFalse(_cache.ContainsKey(Guid.NewGuid()));
        }

        [TestMethod]
        public void ContainsKey_ReturnsTrue_IfCacheDoesContainKey()
        {
            var key = Guid.NewGuid();

            _cache.Add(key, new object());

            Assert.IsTrue(_cache.ContainsKey(key));
        }

        [TestMethod]
        public void Contains_ReturnsFalse_IfCacheDoesNotContainKeyValuePair()
        {
            Assert.IsFalse(_cache.Contains(new KeyValuePair<Guid, object>(Guid.NewGuid(), new object())));
        }

        [TestMethod]
        public void Contains_ReturnsTrue_IfCacheDoesContainKeyValuePair()
        {
            var key = Guid.NewGuid();
            var value = new object();

            _cache.Add(key, value);

            Assert.IsTrue(_cache.Contains(new KeyValuePair<Guid, object>(key, value)));
        }

        [TestMethod]
        public void TryGetValue_ReturnsFalseAndAssignsNull_IfCacheDoesNotContainKey()
        {
            object value;

            Assert.IsFalse(_cache.TryGetValue(Guid.NewGuid(), out value));
            Assert.IsNull(value);
        }

        [TestMethod]
        public void TryGetValue_ReturnsTrueAndAssignsValue_IfCacheDoesContainKey()
        {
            var key = Guid.NewGuid();
            var value = new object();

            _cache.Add(key, value);

            object assignedValue;

            Assert.IsTrue(_cache.TryGetValue(key, out assignedValue));
            Assert.AreSame(value, assignedValue);
        }

        #endregion
    }
}
