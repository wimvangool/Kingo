namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class MemberConstraint<TValueIn, TValueOut>
    {
        private readonly MemberByTransformation _member;
        private readonly IConstraint<TValueIn, TValueOut> _constraint;        

        internal MemberConstraint(MemberByTransformation member, IConstraint<TValueIn, TValueOut> constraint)
        {
            _member = member;
            _constraint = constraint;            
        }        

        internal bool IsNotSatisfiedBy(TValueIn value, out IErrorMessage errorMessage)
        {
            if (_constraint.IsNotSatisfiedBy(value, out errorMessage))
            {
                errorMessage.Add(ErrorMessage.MemberIdentifier, _member.WithValue(errorMessage.FailedValue));
                return true;
            }
            return false;
        }
    }
}
