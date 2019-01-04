using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Domain
{
    internal sealed class NumberDeletedEvent : NumberSnapshotOrEvent
    {
        public NumberDeletedEvent(Guid id, int version) :
            base(id, version) { }        
    }
}
