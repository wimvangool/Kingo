using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class ConstraintWrapper<TValue> : Constraint<TValue>
    {
        private readonly IConstraint<TValue> _constraint;

        internal ConstraintWrapper(IConstraint<TValue> constraint, StringTemplate errorMessage = null, Identifier name = null)
            : base(errorMessage, name)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _constraint = constraint;
        }

        #region [====== Name & ErrorMessage ======]

        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new ConstraintWrapper<TValue>(_constraint, ErrorMessage, name);
        }

        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new ConstraintWrapper<TValue>(_constraint, errorMessage, Name);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        public override IConstraint<TValue> And(IConstraint<TValue> constraint)
        {
            return _constraint.And(constraint);
        }

        public override IConstraintWithErrorMessage<TValue> Or(IConstraint<TValue> constraint)
        {
            return _constraint.Or(constraint);
        }

        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintWrapper<TValue>(_constraint.Invert(), errorMessage, name);
        }

        #endregion

        #region [====== MapInputToOutput ======]

        public override IConstraint<TValue, TValue> MapInputToOutput()
        {
            return _constraint.MapInputToOutput();
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        public override bool IsSatisfiedBy(TValue value)
        {
            return _constraint.IsSatisfiedBy(value);
        }

        public override bool IsNotSatisfiedBy(TValue value, out IErrorMessage errorMessage)
        {
            if (IsSatisfiedBy(value))
            {
                errorMessage = null;
                return false;
            }
            errorMessage = new FailedConstraintMessage(this, _constraint);
            return true;
        }

        #endregion
    }
}
