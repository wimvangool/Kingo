using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
    [TestClass]
    public sealed class VirtualCollectionImplementationTest
    {
        private const int _Count = 67;
        private SynchronizationContextScope _scope;
        private VirtualCollectionImplementationSpy _implementation;

        [TestInitialize]
        public void Setup()
        {
            _scope = new SynchronizationContextScope(new SynchronousContext());
            _implementation = new VirtualCollectionImplementationSpy(Enumerable.Range(1, _Count), 10);
        }

        [TestCleanup]
        public void Teardown()
        {
            _scope.Dispose();
        }

        #region [====== TryGetCount ======]

        [TestMethod]
        public void TryGetCount_ReturnsFalse_WhenCountWasNotYetLoaded()
        {
            int count;

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void TryGetCount_LoadsTheCountAsynchronously_IfCountWasNotYetLoaded()
        {
            int countInEventArgs = 0;
            int count;

            _implementation.CountLoaded += (s, e) => countInEventArgs = e.Count;

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.AreEqual(_Count, countInEventArgs);
            Assert.AreEqual(1, _implementation.LoadCountInvocations);
        }

        [TestMethod]
        public void TryGetCount_ReturnsTrue_IfCountWasAlreadyLoaded()
        {
            int count;

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.IsTrue(_implementation.TryGetCount(out count));
            Assert.AreEqual(_Count, count);            
        }

        #endregion

        #region [====== TryGetItem ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryGetItem_Throws_IfCountHasNotBeenLoadedYet()
        {
            int item;

            _implementation.TryGetItem(0, out item);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TryGetItem_Throws_IfIndexIsNegative()
        {
            int count;
            int item;

            _implementation.TryGetCount(out count);
            _implementation.TryGetItem(-1, out item);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TryGetItem_Throws_IfIndexIsTooLarge()
        {
            int count;
            int item;

            Assert.IsFalse(_implementation.TryGetCount(out count));

            _implementation.TryGetItem(_Count, out item);
        }

        [TestMethod]
        public void TryGetItem_ReturnsDefaultValue_IfItemHasNotBeenLoaded()
        {
            int count;            
            int item;

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.IsFalse(_implementation.TryGetItem(19, out item));
            Assert.AreEqual(0, item);            
        }

        [TestMethod]
        public void TryGetItem_RaisesTheItemLoadedEvent_IfItemHasNotBeenLoaded()
        {
            var eventArguments = new List<ItemLoadedEventArgs<int>>();
            int count;            
            int item;

            _implementation.ItemLoaded += (s, e) => eventArguments.Add(e);

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.IsFalse(_implementation.TryGetItem(29, out item));

            Assert.AreEqual(1, _implementation.GetFromCacheInvocations);
            Assert.AreEqual(1, _implementation.LoadPageInvocations);

            Assert.AreEqual(10, eventArguments.Count);
            Assert.AreEqual(20, eventArguments[0].Index);
            Assert.AreEqual(21, eventArguments[1].Index);
            Assert.AreEqual(22, eventArguments[2].Index);
            Assert.AreEqual(23, eventArguments[3].Index);
            Assert.AreEqual(24, eventArguments[4].Index);
            Assert.AreEqual(25, eventArguments[5].Index);
            Assert.AreEqual(26, eventArguments[6].Index);
            Assert.AreEqual(27, eventArguments[7].Index);
            Assert.AreEqual(28, eventArguments[8].Index);
            Assert.AreEqual(29, eventArguments[9].Index);                      
        }

        [TestMethod]
        public void TryGetItem_RetrievesItemFromCache_IfSamePageIsRequestedMultipleTimes()
        {
            int count;
            int item;

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.IsFalse(_implementation.TryGetItem(30, out item));
            Assert.IsTrue(_implementation.TryGetItem(33, out item));
            Assert.IsTrue(_implementation.TryGetItem(35, out item));
            Assert.AreEqual(3, _implementation.GetFromCacheInvocations);
            Assert.AreEqual(1, _implementation.LoadPageInvocations);
        }

        [TestMethod]
        public void TryGetItem_RaisesTheItemLoadedEventTheRightAmountOfTimes_IfLastPageIsLoaded()
        {
            var eventArguments = new List<ItemLoadedEventArgs<int>>();
            int count;
            int item;

            _implementation.ItemLoaded += (s, e) => eventArguments.Add(e);

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.IsFalse(_implementation.TryGetItem(62, out item));
            Assert.IsTrue(_implementation.TryGetItem(66, out item));

            Assert.AreEqual(7, eventArguments.Count);
            Assert.AreEqual(60, eventArguments[0].Index);
            Assert.AreEqual(61, eventArguments[1].Index);
            Assert.AreEqual(62, eventArguments[2].Index);
            Assert.AreEqual(63, eventArguments[3].Index);
            Assert.AreEqual(64, eventArguments[4].Index);
            Assert.AreEqual(65, eventArguments[5].Index);
            Assert.AreEqual(66, eventArguments[6].Index);
        }

        [TestMethod]
        public void TryGetItem_RaisesTheItemFailedToLoadEvent_IfPageFailsToLoad()
        {
            var eventArguments = new List<ItemFailedToLoadEventArgs>();
            int count;
            int item;

            _implementation.FailNextPageLoad = true;
            _implementation.ItemFailedToLoad += (s, e) => eventArguments.Add(e);

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.IsFalse(_implementation.TryGetItem(46, out item));

            Assert.AreEqual(1, _implementation.GetFromCacheInvocations);
            Assert.AreEqual(1, _implementation.LoadPageInvocations);

            Assert.AreEqual(10, eventArguments.Count);
            Assert.AreEqual(40, eventArguments[0].Index);
            Assert.AreEqual(41, eventArguments[1].Index);
            Assert.AreEqual(42, eventArguments[2].Index);
            Assert.AreEqual(43, eventArguments[3].Index);
            Assert.AreEqual(44, eventArguments[4].Index);
            Assert.AreEqual(45, eventArguments[5].Index);
            Assert.AreEqual(46, eventArguments[6].Index);
            Assert.AreEqual(47, eventArguments[7].Index);
            Assert.AreEqual(48, eventArguments[8].Index);
            Assert.AreEqual(49, eventArguments[9].Index);
        }

        [TestMethod]
        public void TryGetItem_CachesTheRequestedPageForALimitedAmountOfTime()
        {
            int count;
            int item;

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.IsFalse(_implementation.TryGetItem(11, out item));
            Assert.IsTrue(_implementation.WaitUntilRemovedFromCache(11, TimeSpan.FromSeconds(1)), "Page was not cached or did not expire.");
        }

        [TestMethod]
        public void TryGetItem_LoadsAPageAgain_IfPreviousPageExpired()
        {
            int count;
            int item;

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.IsFalse(_implementation.TryGetItem(55, out item));
            Assert.IsTrue(_implementation.WaitUntilRemovedFromCache(55, TimeSpan.FromSeconds(1)), "Page was not cached or did not expire.");
            Assert.IsFalse(_implementation.TryGetItem(55, out item), "Page should no longer be in cache.");
        }

        #endregion
    }
}
