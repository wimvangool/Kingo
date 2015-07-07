﻿using System.Threading;

namespace Syztem.ComponentModel.Client
{
    internal sealed class SynchronousContext : SynchronizationContext
    {
        public override void Send(SendOrPostCallback d, object state)
        {
            d.Invoke(state);
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            d.Invoke(state);
        }
    }
}
