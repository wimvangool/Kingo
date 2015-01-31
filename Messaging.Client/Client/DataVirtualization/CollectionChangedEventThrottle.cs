using System.Collections.Specialized;
using System.Windows.Threading;

namespace System.ComponentModel.Client.DataVirtualization
{
    internal sealed class CollectionChangedEventThrottle
    {
        private readonly Action<NotifyCollectionChangedEventArgs> _raiseEvent;
        private readonly DispatcherTimer _timer;
        private NotifyCollectionChangedEventArgs _lastEvent;

        internal CollectionChangedEventThrottle(Action<NotifyCollectionChangedEventArgs> raiseEvent)
        {
            _raiseEvent = raiseEvent;
            _timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);            
            _timer.Tick += HandleTimerTick;
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
            _lastEvent = e;

            if (_timer.IsEnabled)
            {
                _timer.Stop();
            }
            _timer.Start();
        }

        private void HandleTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();

            if (_lastEvent == null)
            {
                return;
            }            
            _raiseEvent.Invoke(_lastEvent);
        }

        internal static readonly TimeSpan DefaultRaiseInterval = TimeSpan.FromMilliseconds(100);
    }
}
