using System.Collections.Generic;
using System.ComponentModel.Client.DataVirtualization;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.WpfApplication.Shell
{
    internal sealed class IntegerItemLoader : VirtualCollectionPageLoader<int>
    {
        private static readonly MemoryCache _Cache = new MemoryCache("Pages");
        private const int _CollectionSize = 3456789;
        private const int _PageSize = 10;

        internal readonly string Name;

        internal IntegerItemLoader(string name)
            : base(Guid.NewGuid(), _PageSize)
        {
            Name = name;
        }

        protected override Task<int> StartLoadCountTask()
        {
            return Task<int>.Factory.StartNew(() =>
            {
                // We simulate a load delay here.
                Thread.Sleep(TimeSpan.FromMilliseconds(10));

                return _CollectionSize;
            });
        }

        protected override Task<IList<int>> StartLoadPageTask(int pageIndex)
        {
            return Task<IList<int>>.Factory.StartNew(() =>
            {
                // We simulate a load delay here.
                Thread.Sleep(TimeSpan.FromMilliseconds(20));

                return Enumerable.Range(FirstItemOfPage(pageIndex), PageSize).ToArray();
            });
        }

        protected override ObjectCache PageCache
        {
            get { return _Cache; }
        }

        protected override CacheItemPolicy CreatePageCachePolicy(int pageIndex)
        {
            return new CacheItemPolicy()
            {
                SlidingExpiration = TimeSpan.FromSeconds(5),
                RemovedCallback = args =>                                  
                    Debug.WriteLine("Page {0} was removed from cache.", pageIndex)                
            };
        }

        private int FirstItemOfPage(int pageIndex)
        {
            return pageIndex * PageSize + 1;
        }

        protected override void OnCountLoaded(CountLoadedEventArgs e)
        {
            base.OnCountLoaded(e);

            Debug.WriteLine("{0}: Count Loaded ({1})", Name, e.Count);
        }

        protected override void OnPageLoaded(PageLoadedEventArgs<int> e)
        {
            base.OnPageLoaded(e);

            Debug.WriteLine("{0}: Page Loaded (Index = {1}, Count = {2})", Name, e.Page.PageIndex, e.Page.Count);
        }
    }
}
