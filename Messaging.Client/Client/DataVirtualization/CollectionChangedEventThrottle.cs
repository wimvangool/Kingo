using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace System.ComponentModel.Client.DataVirtualization
{
    internal sealed class CollectionChangedEventThrottle
    {
        private readonly Action<NotifyCollectionChangedEventArgs> _raiseEvent;
        private readonly DispatcherTimer _timer;
        private readonly Queue<NotifyCollectionChangedEventArgs> _events;

        internal CollectionChangedEventThrottle(Action<NotifyCollectionChangedEventArgs> raiseEvent)
        {
            _raiseEvent = raiseEvent;
            _timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);            
            _timer.Tick += HandleTimerTick;
            _events = new Queue<NotifyCollectionChangedEventArgs>();
        }

        internal TimeSpan RaiseInterval
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        internal void NotifyCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            _events.Enqueue(e);

            if (_timer.IsEnabled)
            {
                _timer.Stop();
            }
            _timer.Start();
        }

        private void HandleTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();

            if (_events.Count == 0)
            {
                return;
            }
            _raiseEvent.Invoke(Flush(_events));            
        }

        private static NotifyCollectionChangedEventArgs Flush(Queue<NotifyCollectionChangedEventArgs> events)
        {
            // In the future, it would be better to actually merge the events
            // based on their content. However, for now we simply empty the queue
            // and return the last event.
            NotifyCollectionChangedEventArgs lastEvent;

            do
            {
                lastEvent = events.Dequeue();
            }
            while (events.Count > 0);

            return lastEvent;
        }

        internal static readonly TimeSpan DefaultRaiseInterval = TimeSpan.FromMilliseconds(100);
    }
}
