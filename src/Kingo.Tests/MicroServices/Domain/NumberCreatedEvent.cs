using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Domain
{
    public abstract class NumberCreatedEvent : NumberSnapshotOrEvent
    {
        protected NumberCreatedEvent(Guid id, int value) :
            base(id, 1)
        {            
            Value = value;
        }       

        public int Value
        {
            get;
        }        
    }
}
