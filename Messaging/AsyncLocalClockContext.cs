﻿using System.Diagnostics;
using System.Threading;

namespace System
{
    internal sealed class AsyncLocalClockContext : IClockContext
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly AsyncLocal<IClock> _clock;
        
        private AsyncLocalClockContext()
        {
            _clock = new AsyncLocal<IClock>(StaticClockContext.Instance);
        }
    
        public IClock CurrentClock
        {
            get { return _clock.Value; }
            set { _clock.Value = value; }
        }

        internal static readonly AsyncLocalClockContext Instance = new AsyncLocalClockContext();
    }
}
