using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Kingo.MicroServices.Domain
{
    internal abstract class Number : AggregateRoot<Guid, int>
    {                             
        protected Number(IEventBus eventBus, NumberSnapshot snapshot)
            : base(eventBus, snapshot, false)
        {
            Value = snapshot.Value;
        }

        protected Number(IEventBus eventBus, NumberCreatedEvent @event, bool isNewAggregate)
            : base(eventBus, @event, isNewAggregate)
        {
            Value = @event.Value;
        }

        protected override int NextVersion() =>
            Version + 1;

        protected int Value
        {
            get;
            set;
        }

        public abstract void Add(int value);             
    }
}
