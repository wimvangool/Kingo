using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class ConstraintImplementation<TValueIn, TValueOut> : ConstraintImplementationBase<TValueIn>        
    {
        private readonly IConstraint<TValueIn, TValueOut> _constraint;

        internal ConstraintImplementation(IConstraint<TValueIn, TValueOut> constraint)
        {
            _constraint = constraint;
        }

        protected override IConstraint<TValueIn> Constraint
        {
            get { return _constraint; }
        }

        #region [====== And, Or & Invert ======]        

        public IConstraint<TValueIn, TResult> And<TResult>(IConstraint<TValueOut, TResult> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return new AndConstraint<TValueIn, TValueOut, TResult>(_constraint, constraint);
        }        

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public bool IsSatisfiedBy(TValueIn value)
        {
            TValueOut valueOut;

            return _constraint.IsSatisfiedBy(value, out valueOut);
        }        

        /// <inheritdoc />
        public bool IsNotSatisfiedBy(TValueIn value, out IErrorMessage errorMessage)
        {
            TValueOut valueOut;

            return _constraint.IsNotSatisfiedBy(value, out errorMessage, out valueOut);
        }        

        #endregion
    }
}
