using System;

namespace Kingo.Messaging.Validation
{
    internal sealed class ConstraintWrapper<TValue> : Constraint<TValue>
    {
        private readonly IConstraint<TValue> _constraint;

        internal ConstraintWrapper(IConstraint<TValue> constraint)            
        {            
            _constraint = constraint ?? throw new ArgumentNullException(nameof(constraint));
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

        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name) => new ConstraintWrapper<TValue>(this, name);

        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage) => new ConstraintWrapper<TValue>(this, errorMessage);

        #endregion

        #region [====== Visitor ======]
        
        public override void AcceptVisitor(IConstraintVisitor visitor)
        {
            _constraint.AcceptVisitor(visitor);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        public override IConstraint<TValue> And(IConstraint<TValue> constraint) => _constraint.And(constraint);

        public override IConstraintWithErrorMessage<TValue> Or(IConstraint<TValue> constraint) => _constraint.Or(constraint);

        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null) => new ConstraintWrapper<TValue>(_constraint.Invert()).WithErrorMessage(errorMessage).WithName(name);

        #endregion

        #region [====== Conversion ======]

        public override IFilter<TValue, TValue> MapInputToOutput() => _constraint.MapInputToOutput();

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        public override bool IsSatisfiedBy(TValue value) => _constraint.IsSatisfiedBy(value);

        #endregion
    }
}
