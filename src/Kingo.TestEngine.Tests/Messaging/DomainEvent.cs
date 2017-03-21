using Kingo.Messaging.Validation;

namespace Kingo.Messaging
{
    internal sealed class DomainEvent : Message
    {
        public readonly int Value;

        public DomainEvent(int value = 0)
        {
            Value = value;
        }

        private DomainEvent(DomainEvent message)
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

        public override Message Copy()
        {
            return new DomainEvent(this);
        }

        public override string ToString()
        {
            return string.Format("DomainEvent ({0}).", Value);
        }
    }
}
