using System;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a lifetime of fixed length.
    /// </summary>
    public class FixedLifetime : TimerBasedLifetime
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedLifetime" /> class.
        /// </summary>        
        /// <param name="timeout">The length of the lifetime.</param>        
        public FixedLifetime(TimeSpan timeout) : base(timeout) { }             

        /// <summary>
        /// Starts the timer if not already started.
        /// </summary>
        public override void NotifyValueAccessed()
        {
            if (Timer.Enabled)
            {
                return;
            }
            Timer.Start();
        }         
    }
}
