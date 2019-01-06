using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Domain
{
    internal abstract class NumberSnapshot : NumberSnapshotOrEvent
    {
        protected NumberSnapshot(Guid id, int version, int value, bool hasBeenRemoved = false)
            : base(id, version)
        {            
            Value = value;
            HasBeenRemoved = hasBeenRemoved;
        }        

        public int Value
        {
            get;
        }   
        
        public bool HasBeenRemoved
        {
            get;
        }
    }
}
