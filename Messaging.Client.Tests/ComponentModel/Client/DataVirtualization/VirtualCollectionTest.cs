using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Client.DataVirtualization
{
    [TestClass]
    public sealed class VirtualCollectionTest
    {
        private const int _Count = 67;        
        private VirtualCollectionSpy _collection;

        [TestInitialize]
        public void Setup()
        {                        
            _collection = new VirtualCollectionSpy(Enumerable.Range(1, _Count), 10);
        }

        [TestCleanup]
        public void Teardown()
        {
            _collection.Dispose();            
        }

        #region [====== Count ======]

        [TestMethod]
        public void Count_ReturnsZero_IfCountHasNotBeenLoaded()
        {            
            Assert.AreEqual(0, _collection.Count);
        }

        [TestMethod]
        public void Count_ReturnsCount_IfCountHasBeenLoaded()
        {            
            Assert.AreEqual(0, _collection.Count);

            if (_collection.WaitForCollectionChangedEvent(TimeSpan.FromMilliseconds(100)))
            {
                Assert.AreEqual(_Count, _collection.Count);
                return;
            }
            Assert.Fail("Count was not loaded");
        }

        #endregion

        #region [====== Indexer ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Indexer_Throws_IfCountHasNotBeenLoaded()
        {
            Assert.Fail("Item should not have been loaded: {0}.", _collection[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Indexer_Throws_IfIndexIsLargerThanOrEqualToCount()
        {
            Assert.AreEqual(0, _collection.Count);
            Assert.Fail("Item should not have been loaded: {0}.", _collection[67]);
        }

        [TestMethod]
        public void Indexer_ReturnsNotLoadedItem_IfPageHasNotBeenLoaded()
        {
            Assert.AreEqual(0, _collection.Count);

            var item = _collection[12];

            Assert.IsTrue(item.IsNotLoaded);
            Assert.IsFalse(item.IsLoaded);
            Assert.IsFalse(item.FailedToLoad);
            Assert.AreEqual(0, item.Value);
        }

        [TestMethod]
        public void Indexer_ReturnsLoadedItem_IfPageHasBeenLoaded()
        {
            Assert.AreEqual(0, _collection.Count);

            const int itemIndex = 9;
            var item = new VirtualCollectionItem<int>(itemIndex, VirtualCollectionItemStatus.NotLoaded);

            _collection.PageLoaded += (s, e) => item = _collection[itemIndex];
            _collection.Loader.LoadPage(0);

            Assert.IsFalse(item.IsNotLoaded);
            Assert.IsTrue(item.IsLoaded);
            Assert.IsFalse(item.FailedToLoad);
            Assert.AreEqual(10, item.Value);
        }

        [TestMethod]
        public void Indexer_ReturnsErrorItem_IfPageCouldNotBeLoaded()
        {
            Assert.AreEqual(0, _collection.Count);

            const int itemIndex = 8;
            var item = new VirtualCollectionItem<int>(itemIndex, VirtualCollectionItemStatus.NotLoaded);

            _collection.PageFailedToLoad += (s, e) => item = _collection[itemIndex];
            _collection.FailNextPageLoad = true;
            _collection.Loader.LoadPage(0);

            Assert.IsFalse(item.IsNotLoaded);
            Assert.IsFalse(item.IsLoaded);
            Assert.IsTrue(item.FailedToLoad);
            Assert.AreEqual(0, item.Value);
        }

        #endregion

        #region [====== GetEnumerator ======]

        private IEnumerable<VirtualCollectionItem<int>> EnumerableCollection
        {
            get { return _collection; }
        }

        [TestMethod]
        public void GetEnumerator_ReturnsEmptySequence_IfCountNotBeenLoaded()
        {
            var enumerator = EnumerableCollection.GetEnumerator();

            Assert.IsNotNull(enumerator);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void GetEnumerator_ReturnsOnlyNotLoadedItems_IfNoPageHasBeenLoaded()
        {
            Assert.AreEqual(0, _collection.Count);

            _collection.WaitForPageLoadSignal = true;

            var enumerator = EnumerableCollection.GetEnumerator();
            int itemCount = 0;

            Assert.IsNotNull(enumerator);

            while (enumerator.MoveNext())
            {
                Assert.IsTrue(enumerator.Current.IsNotLoaded);
                Assert.IsFalse(enumerator.Current.IsLoaded);
                Assert.IsFalse(enumerator.Current.FailedToLoad);
                Assert.AreEqual(0, enumerator.Current.Value);

                itemCount++;
            }
            _collection.SignalPageLoadToContinue();

            Assert.AreEqual(67, itemCount);
        }

        [TestMethod]
        public void GetEnumerator_ReturnsLoadedItemsForPagesThatWereLoaded()
        {
            Assert.AreEqual(0, _collection.Count);

            _collection.Loader.LoadPage(1);
            _collection.WaitForPageLoadSignal = true;

            var enumerator = EnumerableCollection.GetEnumerator();
            int itemCount = 0;

            Assert.IsNotNull(enumerator);

            while (enumerator.MoveNext())
            {
                // Page 1 contains items 10 to 19, which should be loaded.
                if (10 <= itemCount && itemCount <= 19)
                {
                    Assert.IsFalse(enumerator.Current.IsNotLoaded);
                    Assert.IsTrue(enumerator.Current.IsLoaded);
                    Assert.IsFalse(enumerator.Current.FailedToLoad);
                    Assert.AreEqual(itemCount + 1, enumerator.Current.Value);
                }
                else
                {
                    Assert.IsTrue(enumerator.Current.IsNotLoaded);
                    Assert.IsFalse(enumerator.Current.IsLoaded);
                    Assert.IsFalse(enumerator.Current.FailedToLoad);
                    Assert.AreEqual(0, enumerator.Current.Value);
                }
                itemCount++;
            }
            _collection.SignalPageLoadToContinue();

            Assert.AreEqual(67, itemCount);
        }

        [TestMethod]
        public void GetEnumerator_ReturnsErrorItemsForPagesThatFailedLoad()
        {
            Assert.AreEqual(0, _collection.Count);

            _collection.FailNextPageLoad = true;
            _collection.Loader.LoadPage(2);
            _collection.FailNextPageLoad = false;
            _collection.WaitForPageLoadSignal = true;

            var enumerator = EnumerableCollection.GetEnumerator();
            int itemCount = 0;

            Assert.IsNotNull(enumerator);

            while (enumerator.MoveNext())
            {
                // Page 2 contains items 20 to 29, which should have failed to load.
                if (20 <= itemCount && itemCount <= 29)
                {
                    Assert.IsFalse(enumerator.Current.IsNotLoaded);
                    Assert.IsFalse(enumerator.Current.IsLoaded);
                    Assert.IsTrue(enumerator.Current.FailedToLoad, "Item should have failed to load.");
                    Assert.AreEqual(0, enumerator.Current.Value);
                }
                else
                {
                    Assert.IsTrue(enumerator.Current.IsNotLoaded);
                    Assert.IsFalse(enumerator.Current.IsLoaded);
                    Assert.IsFalse(enumerator.Current.FailedToLoad);
                    Assert.AreEqual(0, enumerator.Current.Value);
                }
                itemCount++;
            }
            _collection.SignalPageLoadToContinue();

            Assert.AreEqual(_Count, itemCount);
        }

        #endregion

        #region [====== TryGetCount ======]

        [TestMethod]
        public void TryGetCount_ReturnsFalse_WhenCountWasNotYetLoaded()
        {
            int count;

            Assert.IsFalse(_collection.Loader.TryGetCount(false, out count));
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void TryGetCount_LoadsTheCountAsynchronously_IfCountWasNotYetLoaded()
        {
            int countInEventArgs = 0;
            int count;

            _collection.CountLoaded += (s, e) => countInEventArgs = e.Count;

            Assert.IsFalse(_collection.Loader.TryGetCount(true, out count));
            Assert.AreEqual(_Count, countInEventArgs);
            Assert.AreEqual(1, _collection.LoadCountInvocations);
        }

        [TestMethod]
        public void TryGetCount_ReturnsTrue_IfCountWasAlreadyLoaded()
        {
            int count;

            Assert.IsFalse(_collection.Loader.TryGetCount(true, out count));
            Assert.IsTrue(_collection.Loader.TryGetCount(false, out count));
            Assert.AreEqual(_Count, count);            
        }

        #endregion

        #region [====== TryGetItem ======]

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryGetItem_Throws_IfCountHasNotBeenLoadedYet()
        {
            int item;

            _collection.Loader.TryGetItem(0, false, out item);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TryGetItem_Throws_IfIndexIsNegative()
        {
            int count;
            int item;

            _collection.Loader.TryGetCount(true, out count);
            _collection.Loader.TryGetItem(-1, false, out item);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TryGetItem_Throws_IfIndexIsTooLarge()
        {
            int count;
            int item;

            Assert.IsFalse(_collection.Loader.TryGetCount(true, out count));

            _collection.Loader.TryGetItem(_Count, false, out item);
        }

        [TestMethod]
        public void TryGetItem_ReturnsDefaultValue_IfItemHasNotBeenLoaded()
        {
            int count;            
            int item;

            Assert.IsFalse(_collection.Loader.TryGetCount(true, out count));
            Assert.IsFalse(_collection.Loader.TryGetItem(19, false, out item));
            Assert.AreEqual(0, item);            
        }

        [TestMethod]
        public void TryGetItem_RaisesThePageLoadedEvent_IfPageHasNotBeenLoaded()
        {            
            int count;            
            int item;
            PageLoadedEventArgs<int> page = null;

            _collection.PageLoaded += (s, e) => page = e;

            Assert.IsFalse(_collection.Loader.TryGetCount(true, out count));
            Assert.IsFalse(_collection.Loader.TryGetItem(29, true, out item));
            
            Assert.AreEqual(1, _collection.LoadPageInvocations);

            Assert.IsNotNull(page);     
            Assert.AreEqual(2, page.PageIndex);
            Assert.IsTrue(page.HasPreviousPage);
            Assert.IsTrue(page.HasNextPage);

            Assert.IsTrue(_collection.Loader.HasLoadedItem(29));
            Assert.IsTrue(_collection.Loader.HasLoadedPage(2));
            Assert.IsFalse(_collection.Loader.HasFailedToLoadItem(29));
            Assert.IsFalse(_collection.Loader.HasFailedToLoadPage(2));
        }

        [TestMethod]
        public void TryGetItem_RetrievesItemFromCache_IfSamePageIsRequestedMultipleTimes()
        {
            var pages = new List<PageLoadedEventArgs<int>>();
            int count;
            int item;

            _collection.PageLoaded += (s, e) => pages.Add(e);

            Assert.IsFalse(_collection.Loader.TryGetCount(true, out count));
            Assert.IsFalse(_collection.Loader.TryGetItem(30, true, out item));
            Assert.IsTrue(_collection.Loader.TryGetItem(33, true, out item));
            Assert.IsTrue(_collection.Loader.TryGetItem(35, true, out item));
            
            Assert.AreEqual(3, _collection.LoadPageInvocations);
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

            Assert.IsTrue(_collection.Loader.HasLoadedItem(30));
            Assert.IsTrue(_collection.Loader.HasLoadedItem(33));
            Assert.IsTrue(_collection.Loader.HasLoadedItem(35));

            Assert.IsFalse(_collection.Loader.HasFailedToLoadItem(30));
            Assert.IsFalse(_collection.Loader.HasFailedToLoadItem(33));
            Assert.IsFalse(_collection.Loader.HasFailedToLoadItem(35));

            Assert.IsFalse(_collection.Loader.HasLoadedPage(0));
            Assert.IsFalse(_collection.Loader.HasLoadedPage(1));
            Assert.IsTrue(_collection.Loader.HasLoadedPage(2));
            Assert.IsTrue(_collection.Loader.HasLoadedPage(3));
            Assert.IsTrue(_collection.Loader.HasLoadedPage(4));
            Assert.IsFalse(_collection.Loader.HasLoadedPage(5));
        }

        [TestMethod]
        public void TryGetItem_RaisesTheItemLoadedEventTheRightAmountOfTimes_IfLastPageIsLoaded()
        {            
            int count;
            int item;
            PageLoadedEventArgs<int> page = null;

            _collection.PageLoaded += (s, e) => page = e;

            Assert.IsFalse(_collection.Loader.TryGetCount(true, out count));
            Assert.IsFalse(_collection.Loader.TryGetItem(62, true, out item));
            Assert.IsTrue(_collection.Loader.TryGetItem(66, true, out item));
            
            Assert.AreEqual(1, _collection.LoadPageInvocations);

            Assert.IsNotNull(page);
            Assert.AreEqual(6, page.PageIndex);
            Assert.IsTrue(page.HasPreviousPage);
            Assert.IsFalse(page.HasNextPage);

            Assert.IsFalse(_collection.Loader.HasLoadedPage(5));
            Assert.IsTrue(_collection.Loader.HasLoadedPage(6));
            Assert.IsFalse(_collection.Loader.HasLoadedPage(7));
        }

        [TestMethod]
        public void TryGetItem_RaisesTheItemFailedToLoadEvent_IfPageFailsToLoad()
        {            
            int count;
            int item;
            int? pageIndex = null;

            _collection.FailNextPageLoad = true;
            _collection.PageFailedToLoad += (s, e) => pageIndex = e.PageIndex;

            Assert.IsFalse(_collection.Loader.TryGetCount(true, out count));
            Assert.IsFalse(_collection.Loader.TryGetItem(46, true, out item));
            
            Assert.AreEqual(1, _collection.LoadPageInvocations);     
       
            Assert.IsTrue(pageIndex.HasValue);
            Assert.AreEqual(4, pageIndex.Value);

            Assert.IsFalse(_collection.Loader.HasLoadedPage(4));
            Assert.IsTrue(_collection.Loader.HasFailedToLoadPage(4));
        }

        [TestMethod]
        public void TryGetItem_CachesTheRequestedPageForALimitedAmountOfTime()
        {
            int count;
            int item;

            Assert.IsFalse(_collection.Loader.TryGetCount(true, out count));
            Assert.IsFalse(_collection.Loader.TryGetItem(11, true, out item));
            Assert.IsTrue(_collection.WaitUntilRemovedFromCache(11, TimeSpan.FromSeconds(20)), "Page was not cached or did not expire.");
        }

        [TestMethod]
        public void TryGetItem_LoadsAPageAgain_IfPreviousPageExpired()
        {
            int count;
            int item;

            Assert.IsFalse(_collection.Loader.TryGetCount(true, out count));
            Assert.IsFalse(_collection.Loader.TryGetItem(55, true, out item));
            Assert.IsTrue(_collection.WaitUntilRemovedFromCache(55, TimeSpan.FromSeconds(30)), "Page was not cached or did not expire.");
            Assert.IsFalse(_collection.Loader.TryGetItem(55, false, out item), "Page should no longer be in cache.");
        }

        #endregion
    }
}
