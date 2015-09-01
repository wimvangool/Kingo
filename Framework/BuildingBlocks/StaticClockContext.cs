
using System;

namespace Kingo.BuildingBlocks
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

        DateTimeOffset IClock.UtcDate()
        {
            return CurrentClock.UtcDate();
        }

        DateTimeOffset IClock.UtcDateAndTime()
        {
            return CurrentClock.UtcDateAndTime();
        }

        TimeSpan IClock.LocalTime()
        {
            return CurrentClock.LocalTime();
        }

        DateTimeOffset IClock.LocalDate()
        {
            return CurrentClock.LocalDate();
        }

        DateTimeOffset IClock.LocalDateAndTime()
        {
            return CurrentClock.LocalDateAndTime();
        }

        public IClock Add(TimeSpan offset)
        {
            return ClockWithOffset.AddOffset(this, offset);
        }        

        public static readonly StaticClockContext Instance = new StaticClockContext();        
    }
}
