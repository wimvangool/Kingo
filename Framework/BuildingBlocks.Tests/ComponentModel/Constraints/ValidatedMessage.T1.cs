namespace Kingo.BuildingBlocks.ComponentModel.Constraints
{   
    internal sealed class ValidatedMessage<TValue> : Message<ValidatedMessage<TValue>>
    {
        internal readonly TValue Member;        

        internal ValidatedMessage(TValue member)
        {
            Member = member;
        }

        private ValidatedMessage(ValidatedMessage<TValue> message)
            : base(message)
        {
            Member = message.Member;
        }

        public override ValidatedMessage<TValue> Copy()
        {
            return new ValidatedMessage<TValue>(this);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ValidatedMessage<TValue>);
        }

        private bool Equals(ValidatedMessage<TValue> other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return Equals(Member, other.Member);
        }

        public override int GetHashCode()
        {
            return HashCode.Of(Member);
        }
    }   
}
