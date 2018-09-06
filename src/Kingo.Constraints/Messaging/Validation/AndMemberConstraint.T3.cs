namespace Kingo.Messaging.Validation
{
    internal sealed class AndMemberConstraint<TValueIn, TMiddle, TValueOut> : IMemberConstraint<TValueIn, TValueOut>
    {
        private readonly IMemberConstraint<TValueIn, TMiddle> _leftConstraint;
        private readonly IMemberConstraint<TMiddle, TValueOut> _rightConstraint;

        internal AndMemberConstraint(IMemberConstraint<TValueIn, TMiddle> leftConstraint, IMemberConstraint<TMiddle, TValueOut> rightConstraint)
        {
            _leftConstraint = leftConstraint;
            _rightConstraint = rightConstraint;
        }

        public IMemberConstraint<TValueIn, TOther> And<TOther>(IMemberConstraint<TValueOut, TOther> constraint) =>
             new AndMemberConstraint<TValueIn, TValueOut, TOther>(this, constraint);

        public bool IsNotSatisfiedBy(Member<TValueIn> member, IErrorMessageCollection reader, out Member<TValueOut> transformedMember)
        {
            if (_leftConstraint.IsNotSatisfiedBy(member, reader, out var middleMember))
            {
                transformedMember = null;
                return true;
            }
            return _rightConstraint.IsNotSatisfiedBy(middleMember, reader, out transformedMember);
        }
    }
}
