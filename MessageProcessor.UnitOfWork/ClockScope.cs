using System;
using YellowFlare.MessageProcessing.Resources;

namespace YellowFlare.MessageProcessing
{
    internal sealed class ClockScope : IDisposable
    {
        private readonly IClockContext _context;
        private readonly IClock _previousClock;
        private readonly IClock _currentClock;
        private bool _isDisposed;

        public ClockScope(IClockContext context, IClock clock)
        {
            _context = context;
            _previousClock = context.CurrentClock;
            _currentClock = context.CurrentClock = clock;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            RestorePreviousClock();

            _isDisposed = true;
        }

        private void RestorePreviousClock()
        {
            if (_currentClock != _context.CurrentClock)
            {
                throw NewIncorrectNestingOfScopesException();
            }
            _context.CurrentClock = _previousClock;
        }

        private static Exception NewIncorrectNestingOfScopesException()
        {
            return new InvalidOperationException(ExceptionMessages.ClockScope_IncorrectNesting);
        }
    }
}
