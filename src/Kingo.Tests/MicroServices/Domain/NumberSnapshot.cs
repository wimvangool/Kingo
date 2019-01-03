using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Domain
{
    internal abstract class NumberSnapshot : NumberSnapshotOrEvent
    {
        protected NumberSnapshot(Guid id, int version, int value)
            : base(id, version)
        {            
            Value = value;
        }        

        public int Value
        {
            get;
        }        
    }
}
