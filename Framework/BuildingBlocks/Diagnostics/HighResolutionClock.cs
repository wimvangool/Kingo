using System;
using System.Diagnostics;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Kingo.BuildingBlocks.Diagnostics
{
    /// <summary>
    /// Represents a clock that uses a <see cref="Stopwatch" /> in combination with
    /// the system clock to simulate a high resolution system time clock.
    /// </summary>
    public sealed class HighResolutionClock : Clock, IDisposable
    {
        private readonly object _lock = new object();
        private readonly IClock _referenceClock;
        private readonly Timer _synchronizationTimer;

        private StopwatchClock _stopwatchClock;
        private long _lastTimestampTicks;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighResolutionClock" /> class.
        /// </summary>
        public HighResolutionClock()
            : this(SystemClock.Instance) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HighResolutionClock" /> class.
        /// </summary>
        /// <param name="referenceClock">The clock that is used to obtain the system time.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="referenceClock "/> is <c>null</c>.
        /// </exception>       
        public HighResolutionClock(IClock referenceClock)
            : this(referenceClock, TimeSpan.FromSeconds(10)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HighResolutionClock" /> class.
        /// </summary>
        /// <param name="referenceClock">
        /// The clock that will be used as a reference for this clock.
        /// </param>
        /// <param name="synchronizationInterval">
        /// The interval that is used to periodically synchronize this clock with the specified <paramref name="referenceClock"/>.
        /// A value of <see cref="TimeSpan.Zero" /> means this clock is never synchronized.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="referenceClock "/> is <c>null</c>.
        /// </exception>
        public HighResolutionClock(IClock referenceClock, TimeSpan synchronizationInterval)
        {
            if (referenceClock == null)
            {
                throw new ArgumentNullException("referenceClock");
            }
            _referenceClock = referenceClock;

            if (synchronizationInterval.Equals(TimeSpan.Zero))
            {
                _synchronizationTimer = null;
            }
            else
            {
                _synchronizationTimer = new Timer(synchronizationInterval.TotalMilliseconds);
                _synchronizationTimer.Elapsed += (s, e) => OnSynchronizationRequired();
                _synchronizationTimer.AutoReset = false;
            }                       
        }        

        #region [====== Start, Stop & Dispose ======]

        /// <summary>
        /// Indicates whether or not this clock is running.
        /// </summary>
        public bool IsRunning
        {
            get { return _stopwatchClock != null; }
        }

        /// <summary>
        /// Starts the clock, counting from the date and time provided by the reference clock.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// This clock has already been disposed.
        /// </exception>        
        public void Start()
        {
            if (_isDisposed)
            {
                throw NewObjectDisposedException();
            }
            lock (_lock)
            {
                if (_isDisposed)
                {
                    throw NewObjectDisposedException();
                }
                var clock = Interlocked.Exchange(ref _stopwatchClock, StopwatchClock.StartNew(_referenceClock.UtcDateAndTime()));
                if (clock != null)
                {
                    clock.Stop();
                }
                if (_synchronizationTimer != null)
                {
                    _synchronizationTimer.Start();
                }
            }
        }

        private void OnSynchronizationRequired()
        {
            if (_isDisposed)
            {
                return;
            }
            lock (_lock)
            {
                if (_isDisposed || _stopwatchClock == null)
                {
                    return;
                }
                Interlocked.Exchange(ref _stopwatchClock, StopwatchClock.StartNew(_referenceClock.UtcDateAndTime())).Stop();
            }
        }

        /// <summary>
        /// Stops the clock, freezing the date and time.
        /// </summary>        
        public void Stop()
        {
            if (_isDisposed)
            {
                return;
            }
            lock (_lock)
            {
                if (_isDisposed)
                {
                    return;
                }
                var clock = Interlocked.Exchange(ref _stopwatchClock, null);
                if (clock != null)
                {
                    clock.Stop();
                }
                if (_synchronizationTimer != null)
                {
                    _synchronizationTimer.Stop();
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            lock (_lock)
            {
                if (_isDisposed)
                {
                    return;
                }
                Stop();

                if (_synchronizationTimer != null)
                {
                    _synchronizationTimer.Dispose();
                }
                _isDisposed = true;
            }
        }

        private Exception NewObjectDisposedException()
        {
            return new ObjectDisposedException(GetType().Name);
        }

        #endregion

        /// <inheritdoc />
        public override DateTimeOffset UtcDateAndTime()
        {            
            long oldTimestampTicks;
            long newTimestampTicks;

            do
            {
                var clock = _stopwatchClock;
                if (clock == null)
                {
                    return new DateTimeOffset(_lastTimestampTicks, TimeSpan.Zero);
                }
                oldTimestampTicks = _lastTimestampTicks;
                newTimestampTicks = Math.Max(clock.UtcDateAndTime().Ticks, oldTimestampTicks + 1L);
            }
            while (Interlocked.CompareExchange(ref _lastTimestampTicks, newTimestampTicks, oldTimestampTicks) != oldTimestampTicks);

            return new DateTimeOffset(newTimestampTicks, TimeSpan.Zero);  
        }

        private static readonly Lazy<HighResolutionClock> _Default = new Lazy<HighResolutionClock>(StartNew, true);

        /// <summary>
        /// Returns the default <see cref="HighResolutionClock" /> instance that uses the <see cref="SystemClock" /> as the reference clock.
        /// </summary>
        public static HighResolutionClock Default
        {
            get { return _Default.Value; }
        }

        /// <summary>
        /// Creates and returns a new <see cref="HighResolutionClock" /> that is counting from the current date and time.
        /// </summary>        
        /// <returns>A new <see cref="HighResolutionClock" />.</returns>
        public static HighResolutionClock StartNew()
        {
            var clock = new HighResolutionClock();                
            clock.Start();
            return clock;
        }           
    }
}
