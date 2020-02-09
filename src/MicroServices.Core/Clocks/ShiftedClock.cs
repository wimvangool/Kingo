using System;

namespace Kingo.Clocks
{
    internal sealed class ShiftedClock : Clock
    {
        private readonly IClock _referenceClock;
        private readonly TimeSpan _offset;        

        public ShiftedClock(IClock referenceClock, TimeSpan offset)
        {
            _referenceClock = referenceClock;
            _offset = offset;            
        }

        public override DateTimeOffset LocalDateAndTime() =>
             Shift(_referenceClock.LocalDateAndTime(), _offset);

        public override DateTimeOffset UtcDateAndTime() =>
             Shift(_referenceClock.UtcDateAndTime(), _offset);

        private static DateTimeOffset Shift(DateTimeOffset value, TimeSpan offset) =>
             value.Add(offset);
    }
}
