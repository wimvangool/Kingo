
namespace System.ComponentModel.Messaging
{
    internal sealed class StaticClockContext : IClockContext, IClock
    {        
        private StaticClockContext()
        {
            CurrentClock = SystemClock.Instance;
        }

        public IClock CurrentClock
        {
            get;
            set;
        }

        TimeSpan IClock.UtcTime()
        {
            return CurrentClock.UtcTime();
        }

        DateTime IClock.UtcDate()
        {
            return CurrentClock.UtcDate();
        }

        DateTime IClock.UtcDateAndTime()
        {
            return CurrentClock.UtcDateAndTime();
        }

        TimeSpan IClock.LocalTime()
        {
            return CurrentClock.LocalTime();
        }

        DateTime IClock.LocalDate()
        {
            return CurrentClock.LocalDate();
        }

        DateTime IClock.LocalDateAndTime()
        {
            return CurrentClock.LocalDateAndTime();
        }

        public IClock Add(TimeSpan offset)
        {
            return ClockWithOffset.AddOffset(this, offset);
        }

        public IClock Subtract(TimeSpan offset)
        {
            return ClockWithOffset.SubtractOffset(this, offset);
        }

        public static readonly StaticClockContext Instance = new StaticClockContext();        
    }
}
