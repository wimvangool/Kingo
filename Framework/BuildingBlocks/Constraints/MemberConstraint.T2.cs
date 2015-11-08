namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class MemberConstraint<TValueIn, TValueOut>
    {
        private readonly MemberByTransformation _member;
        private readonly IFilter<TValueIn, TValueOut> _constraint;        

        internal MemberConstraint(MemberByTransformation member, IFilter<TValueIn, TValueOut> constraint)
        {
            _member = member;
            _constraint = constraint;            
        }        

        internal bool IsNotSatisfiedBy(TValueIn value, out IErrorMessage errorMessage)
        {
            if (_constraint.IsNotSatisfiedBy(value, out errorMessage))
            {
                errorMessage.Put(ErrorMessage.MemberIdentifier, _member.WithValue(errorMessage.Value));
                return true;
            }
            return false;
        }
    }
}
