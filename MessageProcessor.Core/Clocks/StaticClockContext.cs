
using System;

namespace YellowFlare.MessageProcessing.Clocks
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

        public static readonly StaticClockContext Instance = new StaticClockContext();        
    }
}
