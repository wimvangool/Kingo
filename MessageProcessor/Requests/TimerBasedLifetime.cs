using System;
using System.Timers;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a lifetime that is based on a <see cref="Timer" />.
    /// </summary>
    public abstract class TimerBasedLifetime : QueryCacheValueLifetime
    {       
        private readonly Lazy<Timer> _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerBasedLifetime" /> class.
        /// </summary>        
        /// <param name="timeout">The length of the lifetime.</param>        
        protected TimerBasedLifetime(TimeSpan timeout)
        {            
            _timer = new Lazy<Timer>(() => CreateTimer(timeout));
        }

        #region [====== Dispose ======]

        protected override void Dispose(bool disposing)
        {
            if (disposing && _timer.IsValueCreated)
            {
                _timer.Value.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion        

        /// <summary>
        /// The timer used to raise the <see cref="IQueryCacheValueLifetime.Expired" /> event.
        /// </summary>
        protected Timer Timer
        {
            get { return _timer.Value; }
        }        

        /// <summary>
        /// Creates and returns a new <see cref="Timer" /> that elapses on the specified <paramref name="timeout"/>.
        /// </summary>
        /// <param name="timeout">
        /// The timeout that determines when the returned <see cref="Timer" /> elapses.
        /// </param>
        /// <returns>A new <see cref="Timer" />.</returns>
        protected virtual Timer CreateTimer(TimeSpan timeout)
        {
            var timer = new Timer(timeout.TotalMilliseconds);
            timer.Elapsed += HandleTimerElapsed;
            timer.AutoReset = false;
            return timer;
        }        

        /// <summary>
        /// Handles the <see cref="System.Timers.Timer.Elapsed" /> event.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>        
        protected virtual void HandleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Timer.Elapsed -= HandleTimerElapsed;
            Timer.Stop();    
                   
            OnIsExpired();
        }  
    }
}
