using System;

namespace Kingo.BuildingBlocks.Messaging
{
    internal sealed class DomainEvent : Message<DomainEvent>       
    {
        public readonly Guid Value;

        public DomainEvent()
        {
            Value = Guid.NewGuid();
        }

        private DomainEvent(DomainEvent message) : base(message)
        {
            Value = message.Value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DomainEvent);
        }

        public bool Equals(DomainEvent other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override DomainEvent Copy()
        {
            return new DomainEvent(this);
        }

        public override string ToString()
        {
            return string.Format("DomainEvent ({0}).", Value);
        }
    }
}
