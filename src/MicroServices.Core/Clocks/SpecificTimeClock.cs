using System;

namespace Kingo.Clocks
{
    internal sealed class SpecificTimeClock : Clock
    {
        private readonly IClock _referenceClock;
        private readonly DateTimeOffset _referenceTimeUtc;
        private readonly DateTimeOffset _currentTimeUtc;

        public SpecificTimeClock(IClock referenceClock, DateTimeOffset value)
        {
            _referenceClock = referenceClock ?? throw new ArgumentNullException(nameof(referenceClock));
            _referenceTimeUtc = referenceClock.UtcDateAndTime();
            _currentTimeUtc = value.ToUniversalTime();
        }

        public override DateTimeOffset UtcDateAndTime() =>
            _currentTimeUtc.Add(TimeThatHasPassed());

        private TimeSpan TimeThatHasPassed() =>
            _referenceClock.UtcDateAndTime().Subtract(_referenceTimeUtc);
    }
}
