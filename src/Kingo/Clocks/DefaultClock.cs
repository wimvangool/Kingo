using System;

namespace Kingo.Clocks
{    
    internal sealed class DefaultClock : Clock
    {        
        /// <inheritdoc />
        public override DateTimeOffset LocalDateAndTime() => DateTimeOffset.Now;

        /// <inheritdoc />
        public override DateTimeOffset UtcDateAndTime() => DateTimeOffset.UtcNow;
    }
}
