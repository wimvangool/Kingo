using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Messaging.Client.DataVirtualization
{
    [TestClass]
    public sealed class VirtualCollectionPageLoaderTest
    {
        private const int _Count = 67;
        private SynchronizationContextScope _scope;
        private VirtualCollectionPageLoaderSpy _implementation;

        [TestInitialize]
        public void Setup()
        {
            _scope = new SynchronizationContextScope(new SynchronousContext());
            _implementation = new VirtualCollectionPageLoaderSpy(Enumerable.Range(1, _Count), 10);
        }

        [TestCleanup]
        public void Teardown()
        {
            _implementation.Dispose();
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
        public void TryGetItem_RaisesThePageLoadedEvent_IfPageHasNotBeenLoaded()
        {            
            int count;            
            int item;
            VirtualCollectionPage<int> page = null;

            _implementation.PageLoaded += (s, e) => page = e.Page;

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.IsFalse(_implementation.TryGetItem(29, out item));
            
            Assert.AreEqual(1, _implementation.LoadPageInvocations);

            Assert.IsNotNull(page);     
            Assert.AreEqual(2, page.PageIndex);
            Assert.IsTrue(page.HasPreviousPage);
            Assert.IsTrue(page.HasNextPage);

            Assert.IsTrue(_implementation.HasLoadedItem(29));
            Assert.IsTrue(_implementation.HasLoadedPage(2));
            Assert.IsFalse(_implementation.HasFailedToLoadItem(29));
            Assert.IsFalse(_implementation.HasFailedToLoadPage(2));
        }

        [TestMethod]
        public void TryGetItem_RetrievesItemFromCache_IfSamePageIsRequestedMultipleTimes()
        {
            var pages = new List<VirtualCollectionPage<int>>();
            int count;
            int item;

            _implementation.PageLoaded += (s, e) => pages.Add(e.Page);

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.IsFalse(_implementation.TryGetItem(30, out item));
            Assert.IsTrue(_implementation.TryGetItem(33, out item));
            Assert.IsTrue(_implementation.TryGetItem(35, out item));
            
            Assert.AreEqual(3, _implementation.LoadPageInvocations);
            Assert.AreEqual(3, pages.Count);
            
            Assert.AreEqual(3, pages[0].PageIndex);
            Assert.IsTrue(pages[0].HasPreviousPage);
            Assert.IsTrue(pages[0].HasNextPage);

            Assert.AreEqual(2, pages[1].PageIndex);
            Assert.IsTrue(pages[1].HasPreviousPage);
            Assert.IsTrue(pages[1].HasNextPage);

            Assert.AreEqual(4, pages[2].PageIndex);
            Assert.IsTrue(pages[2].HasPreviousPage);
            Assert.IsTrue(pages[2].HasNextPage);

            Assert.IsTrue(_implementation.HasLoadedItem(30));
            Assert.IsTrue(_implementation.HasLoadedItem(33));
            Assert.IsTrue(_implementation.HasLoadedItem(35));            

            Assert.IsFalse(_implementation.HasFailedToLoadItem(30));
            Assert.IsFalse(_implementation.HasFailedToLoadItem(33));
            Assert.IsFalse(_implementation.HasFailedToLoadItem(35));            

            Assert.IsFalse(_implementation.HasLoadedPage(0));
            Assert.IsFalse(_implementation.HasLoadedPage(1));
            Assert.IsTrue(_implementation.HasLoadedPage(2));
            Assert.IsTrue(_implementation.HasLoadedPage(3));
            Assert.IsTrue(_implementation.HasLoadedPage(4));
            Assert.IsFalse(_implementation.HasLoadedPage(5));
        }

        [TestMethod]
        public void TryGetItem_RaisesTheItemLoadedEventTheRightAmountOfTimes_IfLastPageIsLoaded()
        {            
            int count;
            int item;
            VirtualCollectionPage<int> page = null;

            _implementation.PageLoaded += (s, e) => page = e.Page;

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.IsFalse(_implementation.TryGetItem(62, out item));
            Assert.IsTrue(_implementation.TryGetItem(66, out item));
            
            Assert.AreEqual(1, _implementation.LoadPageInvocations);

            Assert.IsNotNull(page);
            Assert.AreEqual(6, page.PageIndex);
            Assert.IsTrue(page.HasPreviousPage);
            Assert.IsFalse(page.HasNextPage);

            Assert.IsFalse(_implementation.HasLoadedPage(5));
            Assert.IsTrue(_implementation.HasLoadedPage(6));
            Assert.IsFalse(_implementation.HasLoadedPage(7));
        }

        [TestMethod]
        public void TryGetItem_RaisesTheItemFailedToLoadEvent_IfPageFailsToLoad()
        {            
            int count;
            int item;
            int? pageIndex = null;

            _implementation.FailNextPageLoad = true;
            _implementation.PageFailedToLoad += (s, e) => pageIndex = e.PageIndex;

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.IsFalse(_implementation.TryGetItem(46, out item));
            
            Assert.AreEqual(1, _implementation.LoadPageInvocations);     
       
            Assert.IsTrue(pageIndex.HasValue);
            Assert.AreEqual(4, pageIndex.Value);

            Assert.IsFalse(_implementation.HasLoadedPage(4));
            Assert.IsTrue(_implementation.HasFailedToLoadPage(4));
        }

        [TestMethod]
        public void TryGetItem_CachesTheRequestedPageForALimitedAmountOfTime()
        {
            int count;
            int item;

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.IsFalse(_implementation.TryGetItem(11, out item));
            Assert.IsTrue(_implementation.WaitUntilRemovedFromCache(11, TimeSpan.FromSeconds(20)), "Page was not cached or did not expire.");
        }

        [TestMethod]
        public void TryGetItem_LoadsAPageAgain_IfPreviousPageExpired()
        {
            int count;
            int item;

            Assert.IsFalse(_implementation.TryGetCount(out count));
            Assert.IsFalse(_implementation.TryGetItem(55, out item));
            Assert.IsTrue(_implementation.WaitUntilRemovedFromCache(55, TimeSpan.FromSeconds(30)), "Page was not cached or did not expire.");
            Assert.IsFalse(_implementation.TryGetItem(55, out item), "Page should no longer be in cache.");
        }

        #endregion
    }
}
