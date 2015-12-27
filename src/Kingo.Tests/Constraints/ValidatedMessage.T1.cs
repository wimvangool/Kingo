using System;
using Kingo.Messaging;

namespace Kingo.Constraints
{   
    internal sealed class ValidatedMessage<TValue> : Message<ValidatedMessage<TValue>>
    {
        internal readonly TValue Member;
        internal readonly TValue Other;
        internal readonly TValue Left;
        internal readonly TValue Right;
        internal Type ExpectedMemberType = typeof(TValue);

        internal ValidatedMessage(TValue member)
        {
            Member = member;
        }

        internal ValidatedMessage(TValue member, TValue other)
        {
            Member = member;
            Other = other;
        }

        internal ValidatedMessage(TValue member, TValue left, TValue right)
        {
            Member = member;
            Left = left;
            Right = right;
        }

        private ValidatedMessage(ValidatedMessage<TValue> message)
            : base(message)
        {
            Member = message.Member;
            Other = message.Other;
            Left = message.Left;
            Right = message.Right;
        }

        public override ValidatedMessage<TValue> Copy()
        {
            return new ValidatedMessage<TValue>(this);
        }

        #region [====== Equals & GetHashCode ======]

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
            return
                Equals(Member, other.Member) &&
                Equals(Other, other.Other) &&
                Equals(Left, other.Left) &&
                Equals(Right, other.Right);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        #endregion

        internal ConstraintValidator<ValidatedMessage<TValue>> CreateConstraintValidator(bool haltOnFirstError = false)
        {
            return new ConstraintValidator<ValidatedMessage<TValue>>(haltOnFirstError);
        }
    }   
}
