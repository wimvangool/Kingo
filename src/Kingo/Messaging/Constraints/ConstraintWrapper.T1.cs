using System;

namespace Kingo.Messaging.Constraints
{
    internal sealed class ConstraintWrapper<TValue> : Constraint<TValue>
    {
        private readonly IConstraint<TValue> _constraint;

        internal ConstraintWrapper(IConstraint<TValue> constraint)            
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }
            _constraint = constraint;
        }

        private ConstraintWrapper(ConstraintWrapper<TValue> constraint, StringTemplate errorMessage) 
            : base(constraint, errorMessage)
        {
            _constraint = constraint._constraint;
        }

        private ConstraintWrapper(ConstraintWrapper<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            _constraint = constraint._constraint;
        }

        #region [====== Name & ErrorMessage ======]

        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new ConstraintWrapper<TValue>(this, name);
        }

        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new ConstraintWrapper<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== Visitor ======]
        
        public override void AcceptVisitor(IConstraintVisitor visitor)
        {
            _constraint.AcceptVisitor(visitor);
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
            return new ConstraintWrapper<TValue>(_constraint.Invert()).WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== Conversion ======]

        public override IFilter<TValue, TValue> MapInputToOutput()
        {
            return _constraint.MapInputToOutput();
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        public override bool IsSatisfiedBy(TValue value)
        {
            return _constraint.IsSatisfiedBy(value);
        }        

        #endregion
    }
}
