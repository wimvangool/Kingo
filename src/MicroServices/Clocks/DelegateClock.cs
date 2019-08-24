using System;

namespace Kingo.Clocks
{
    /// <summary>
    /// Represents a clock that implemented by a delegate.
    /// </summary>
    public sealed class DelegateClock : Clock
    {
        private readonly Func<DateTimeOffset> _timeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateClock" /> class.
        /// </summary>
        /// <param name="timeFactory">The delegate that is used to obtain the UTC date and time.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="timeFactory"/> is <c>null</c>.
        /// </exception>
        public DelegateClock(Func<DateTimeOffset> timeFactory)
        {           
            _timeFactory = timeFactory ?? throw new ArgumentNullException(nameof(timeFactory));
        }

        /// <inheritdoc />
        public override DateTimeOffset UtcDateAndTime() =>
            _timeFactory.Invoke().ToUniversalTime();
    }
}
