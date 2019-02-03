using System;

namespace Kingo.Clocks
{
    internal sealed class ShiftedClock : Clock
    {
        private readonly IClock _clock;
        private readonly TimeSpan _offset;        

        private ShiftedClock(IClock clock, TimeSpan offset)
        {
            _clock = clock;
            _offset = offset;            
        }

        public override DateTimeOffset LocalDateAndTime() =>
             Shift(_clock.LocalDateAndTime(), _offset);

        public override DateTimeOffset UtcDateAndTime() =>
             Shift(_clock.UtcDateAndTime(), _offset);

        internal static ShiftedClock Shift(IClock clock, TimeSpan offset) =>
             new ShiftedClock(clock, offset);

        private static DateTimeOffset Shift(DateTimeOffset value, TimeSpan offset) =>
             value.Add(offset);
    }
}
