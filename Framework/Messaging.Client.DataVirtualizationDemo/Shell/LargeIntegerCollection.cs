using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using ServiceComponents.ComponentModel.Client.DataVirtualization;

namespace ServiceComponents.ComponentModel.Client.Shell
{
    internal sealed class LargeIntegerCollection : VirtualCollection<int>
    {        
        private static readonly Random _RandomFailureNumber = new Random();
        private const int _CollectionSize = 3456789;
        private const int _PageSize = 10;

        internal readonly string Name;

        internal LargeIntegerCollection(string name) : base(_PageSize)
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
            var failureNumber = _RandomFailureNumber.NextDouble();

            return Task<IList<int>>.Factory.StartNew(() =>
            {
                // We simulate a load delay here.
                Thread.Sleep(TimeSpan.FromMilliseconds(20));

                if (failureNumber <= 0.1)
                {
                    throw NewRandomPageLoadException();
                }
                return Enumerable.Range(FirstItemOfPage(pageIndex), PageSize).ToArray();
            });
        }

        private static Exception NewRandomPageLoadException()
        {
            return new IOException("Page could not be loaded");
        }        

        protected override CacheItemPolicy CreatePageCachePolicy(int pageIndex, bool isErrorPage)
        {
            if (isErrorPage)
            {
                return new CacheItemPolicy()
                {
                    AbsoluteExpiration = Clock.Current.UtcDateAndTime().AddSeconds(30),
                    RemovedCallback = args =>
                        Debug.WriteLine("ErrorPage {0} was removed from cache.", pageIndex)
                };
            }
            return new CacheItemPolicy()
            {
                SlidingExpiration = TimeSpan.FromSeconds(120),
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

            Debug.WriteLine("{0}: Page Loaded (Index = {1}, Count = {2})", Name, e.PageIndex, e.PageSize);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            Debug.WriteLine("Collection was changed.");
        }
    }
}
