using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Client.DataVirtualization
{
    [TestClass]
    public sealed class VirtualCollectionTest
    {
        private SynchronizationContextScope _scope;
        private VirtualCollectionPageLoaderSpy _implementation;
        private VirtualCollection<int> _collection;

        [TestInitialize]
        public void Setup()
        {
            _scope = new SynchronizationContextScope(new SynchronousContext());
            _implementation = new VirtualCollectionPageLoaderSpy(Enumerable.Range(1, 88), 20);
            _implementation.UseInfiniteCacheLifetime = true;
            _collection = new VirtualCollection<int>(_implementation);
        }

        [TestCleanup]
        public void TearDown()
        {
            _implementation.Dispose();
            _scope.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_IfImplementationIsNull()
        {
            new VirtualCollection<int>(null);
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
            int? count = null;

            _collection.CollectionChanged += (s, e) => count = _collection.Count;

            Assert.AreEqual(0, _collection.Count);
            Assert.AreEqual(88, count);
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
            Assert.Fail("Item should not have been loaded: {0}.", _collection[88]);
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

            VirtualCollectionItem<int> item = VirtualCollectionItem<int>.NotLoadedItem;

            _implementation.PageLoaded += (s, e) => item = _collection[12];
            _implementation.LoadPage(0);           

            Assert.IsFalse(item.IsNotLoaded);
            Assert.IsTrue(item.IsLoaded);
            Assert.IsFalse(item.FailedToLoad);
            Assert.AreEqual(13, item.Value);
        }

        [TestMethod]
        public void Indexer_ReturnsErrorItem_IfPageCouldNotBeLoaded()
        {
            Assert.AreEqual(0, _collection.Count);

            VirtualCollectionItem<int> item = VirtualCollectionItem<int>.NotLoadedItem;

            _implementation.PageFailedToLoad += (s, e) => item = _collection[12];
            _implementation.FailNextPageLoad = true;
            _implementation.LoadPage(0);

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

            _implementation.WaitForPageLoadSignal = true;   

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
            _implementation.SignalPageLoadToContinue();

            Assert.AreEqual(88, itemCount);                        
        }

        [TestMethod]
        public void GetEnumerator_ReturnsLoadedItemsForPagesThatWereLoaded()
        {
            Assert.AreEqual(0, _collection.Count);

            _implementation.LoadPage(1);
            _implementation.WaitForPageLoadSignal = true;            

            var enumerator = EnumerableCollection.GetEnumerator();
            int itemCount = 0;

            Assert.IsNotNull(enumerator);

            while (enumerator.MoveNext())
            {
                // Page 1 contains items 20 to 39, which should be loaded.
                if (20 <= itemCount && itemCount <= 39)
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
            _implementation.SignalPageLoadToContinue();

            Assert.AreEqual(88, itemCount); 
        }

        [TestMethod]
        public void GetEnumerator_ReturnsErrorItemsForPagesThatFailedLoad()
        {
            Assert.AreEqual(0, _collection.Count);

            _implementation.FailNextPageLoad = true;
            _implementation.LoadPage(2);
            _implementation.FailNextPageLoad = false;
            _implementation.WaitForPageLoadSignal = true;

            var enumerator = EnumerableCollection.GetEnumerator();
            int itemCount = 0;

            Assert.IsNotNull(enumerator);

            while (enumerator.MoveNext())
            {
                // Page 1 contains items 40 to 39, which should have failed to load.
                if (40 <= itemCount && itemCount <= 59)
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
            _implementation.SignalPageLoadToContinue();

            Assert.AreEqual(88, itemCount);
        }

        #endregion
    }
}
