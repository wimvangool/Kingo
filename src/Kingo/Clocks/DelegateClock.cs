using System;

namespace Kingo.Clocks
{
    /// <summary>
    /// Represents a clock that implemented by a delegate.
    /// </summary>
    public sealed class DelegateClock : Clock
    {
        private readonly Func<DateTimeOffset> _utcDateAndTimeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateClock" /> class.
        /// </summary>
        /// <param name="utcDateAndTimeFactory">The delegate that is used to obtain the UTC date and time.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="utcDateAndTimeFactory"/> is <c>null</c>.
        /// </exception>
        public DelegateClock(Func<DateTimeOffset> utcDateAndTimeFactory)
        {
            if (utcDateAndTimeFactory == null)
            {
                throw new ArgumentNullException(nameof(utcDateAndTimeFactory));
            }
            _utcDateAndTimeFactory = utcDateAndTimeFactory;
        }

        /// <inheritdoc />
        public override DateTimeOffset UtcDateAndTime()
        {
            return _utcDateAndTimeFactory.Invoke();
        }
    }
}
