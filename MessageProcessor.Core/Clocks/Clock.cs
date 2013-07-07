using System;

namespace YellowFlare.MessageProcessing.Clocks
{
    public abstract class Clock : IClock
    {
        public TimeSpan UtcTime()
        {
            return LocalDateAndTime().ToUniversalTime().TimeOfDay;
        }

        public DateTime UtcDate()
        {
            return LocalDateAndTime().ToUniversalTime().Date;
        }

        public DateTime UtcDateAndTime()
        {
            return LocalDateAndTime().ToUniversalTime();
        }

        public TimeSpan LocalTime()
        {
            return LocalDateAndTime().TimeOfDay;
        }

        public DateTime LocalDate()
        {
            return LocalDateAndTime().Date;
        }

        public abstract DateTime LocalDateAndTime();

        public static IClock Current
        {
            get { return ThreadStaticClockContext.Instance.CurrentClock; }
        }

        public static IDisposable OverrideStatic(IClock clock)
        {
            return new ClockScope(StaticClockContext.Instance, clock);
        }

        public static IDisposable OverrideThreadStatic(IClock clock)
        {
            return new ClockScope(ThreadStaticClockContext.Instance, clock);
        }
    }
}
