namespace System.ComponentModel
{
    internal sealed class ClockWithOffset : Clock
    {
        private readonly IClock _clock;
        private readonly TimeSpan _offset;
        private readonly Func<DateTime, TimeSpan, DateTime> _dateAndTimeCalculator;

        private ClockWithOffset(IClock clock, TimeSpan offset, Func<DateTime, TimeSpan, DateTime> dateAndTimeCalculator)
        {
            _clock = clock;
            _offset = offset;
            _dateAndTimeCalculator = dateAndTimeCalculator;
        }

        public override DateTime LocalDateAndTime()
        {
            return _dateAndTimeCalculator.Invoke(_clock.LocalDateAndTime(), _offset);
        }

        public static ClockWithOffset AddOffset(IClock clock, TimeSpan offset)
        {            
            return new ClockWithOffset(clock, offset, AddOffset);
        }

        private static DateTime AddOffset(DateTime value, TimeSpan offset)
        {
            return value.Add(offset);
        }

        public static ClockWithOffset SubtractOffset(IClock clock, TimeSpan offset)
        {            
            return new ClockWithOffset(clock, offset, SubtractOffset);
        }

        private static DateTime SubtractOffset(DateTime value, TimeSpan offset)
        {
            return value.Subtract(offset);
        }        
    }
}
