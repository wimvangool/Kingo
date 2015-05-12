using System.Diagnostics;

namespace System.ComponentModel
{
    internal sealed class ClockWithOffset : Clock
    {
        private readonly IClock _clock;
        private readonly TimeSpan _offset;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Func<DateTimeOffset, TimeSpan, DateTimeOffset> _dateAndTimeCalculator;

        private ClockWithOffset(IClock clock, TimeSpan offset, Func<DateTimeOffset, TimeSpan, DateTimeOffset> dateAndTimeCalculator)
        {
            _clock = clock;
            _offset = offset;
            _dateAndTimeCalculator = dateAndTimeCalculator;
        }

        public override DateTimeOffset LocalDateAndTime()
        {
            return _dateAndTimeCalculator.Invoke(_clock.LocalDateAndTime(), _offset);
        }

        public static ClockWithOffset AddOffset(IClock clock, TimeSpan offset)
        {            
            return new ClockWithOffset(clock, offset, AddOffset);
        }

        private static DateTimeOffset AddOffset(DateTimeOffset value, TimeSpan offset)
        {
            return value.Add(offset);
        }

        public static ClockWithOffset SubtractOffset(IClock clock, TimeSpan offset)
        {            
            return new ClockWithOffset(clock, offset, SubtractOffset);
        }

        private static DateTimeOffset SubtractOffset(DateTimeOffset value, TimeSpan offset)
        {
            return value.Subtract(offset);
        }        
    }
}
