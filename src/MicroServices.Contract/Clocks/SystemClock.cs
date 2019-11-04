using System;

namespace Kingo.Clocks
{    
    internal sealed class SystemClock : Clock
    {        
        /// <inheritdoc />
        public override DateTimeOffset LocalDateAndTime() =>
             DateTimeOffset.Now;

        /// <inheritdoc />
        public override DateTimeOffset UtcDateAndTime() =>
             DateTimeOffset.UtcNow;
    }
}
