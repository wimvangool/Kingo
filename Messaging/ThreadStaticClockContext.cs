using System.Diagnostics;
using System.Threading;

namespace System
{
    internal sealed class ThreadStaticClockContext : IClockContext
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ThreadLocal<IClock> _clock;
        
        private ThreadStaticClockContext()
        {
            _clock = new ThreadLocal<IClock>(() => StaticClockContext.Instance);
        }
    
        public IClock CurrentClock
        {
            get { return _clock.Value; }
            set { _clock.Value = value; }
        }

        internal static readonly ThreadStaticClockContext Instance = new ThreadStaticClockContext();
    }
}
