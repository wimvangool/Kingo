using System;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a lifetime that is reset or extended each time the associated value is accessed.
    /// </summary>
    public class SlidingWindowLifetime : TimerBasedLifetime
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="SlidingWindowLifetime" /> class.
        /// </summary>        
        /// <param name="timeout">The length of the window.</param>       
        public SlidingWindowLifetime(TimeSpan timeout) : base(timeout) { }    

        /// <summary>
        /// Restarts the timer.
        /// </summary>
        public override void NotifyValueAccessed()
        {
            if (Timer.Enabled)
            {
                Timer.Stop();
            }
            Timer.Start();
        }

        /// <summary>
        /// Returns a lifetime of one minute.
        /// </summary>
        /// <returns>A lifetime of one minute.</returns>
        public static SlidingWindowLifetime DefaultTimeLifetime()
        {
            return new SlidingWindowLifetime(TimeSpan.FromMinutes(1));
        }
    }
}
