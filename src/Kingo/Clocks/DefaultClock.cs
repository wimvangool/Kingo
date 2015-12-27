using System;

namespace Kingo.Clocks
{    
    internal sealed class DefaultClock : Clock
    {        
        /// <inheritdoc />
        public override DateTimeOffset LocalDateAndTime()
        {
            return DateTimeOffset.Now;
        }

        /// <inheritdoc />
        public override DateTimeOffset UtcDateAndTime()
        {
            return DateTimeOffset.UtcNow;
        }                                   
    }
}
