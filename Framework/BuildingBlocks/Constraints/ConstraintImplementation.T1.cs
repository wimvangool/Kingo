namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class ConstraintImplementation<TValue> : ConstraintImplementationBase<TValue>
    {
        private readonly IConstraint<TValue> _constraint;

        internal ConstraintImplementation(IConstraint<TValue> constraint)
        {
            _constraint = constraint;
        }

        protected override IConstraint<TValue> Constraint
        {
            get { return _constraint; }
        }
    }
}
