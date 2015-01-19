using System.Collections.Specialized;
using System.Threading;

namespace System.ComponentModel.Client.DataVirtualization
{
    internal sealed class VirtualCollectionSpy<T> : VirtualCollection<T>, IDisposable
    {
        private readonly ManualResetEventSlim _collectionRaisedEvent;

        internal VirtualCollectionSpy(IVirtualCollectionPageLoader<T> pageLoader)
            : base(pageLoader)
        {
            _collectionRaisedEvent = new ManualResetEventSlim();
        }        

        public void Dispose()
        {
            _collectionRaisedEvent.Dispose();
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            _collectionRaisedEvent.Set();
        }

        internal bool WaitForCollectionChangedEvent(TimeSpan timeout)
        {
            return _collectionRaisedEvent.Wait(timeout);
        }
    }
}
