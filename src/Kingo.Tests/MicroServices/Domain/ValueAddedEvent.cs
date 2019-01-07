using System;

namespace Kingo.MicroServices.Domain
{
    internal sealed class ValueAddedEvent : NumberSnapshotOrEvent
    {
        public ValueAddedEvent(Guid id, int version, int value)
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
