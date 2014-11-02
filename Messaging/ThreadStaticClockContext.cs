using System.Threading;

namespace System.ComponentModel
{
    internal sealed class ThreadStaticClockContext : IClockContext
    {
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

        public static readonly ThreadStaticClockContext Instance = new ThreadStaticClockContext();
    }
}
