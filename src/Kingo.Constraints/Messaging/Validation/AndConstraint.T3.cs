using System;

namespace Kingo.Messaging.Validation
{
    internal sealed class AndConstraint<TValueIn, TValueMiddle, TValueOut> : IFilter<TValueIn, TValueOut>
    {        
        private readonly IFilter<TValueIn, TValueMiddle> _leftConstraint;
        private readonly IFilter<TValueMiddle, TValueOut> _rightConstraint;

        internal AndConstraint(IFilter<TValueIn, TValueMiddle> left, IFilter<TValueMiddle, TValueOut> constraint)            
        {            
            _leftConstraint = left;
            _rightConstraint = constraint ?? throw new ArgumentNullException(nameof(constraint));
        }

        public void AcceptVisitor(IConstraintVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }
            visitor.VisitAnd(this, new IConstraint[] { _leftConstraint, _rightConstraint });
        }

        #region [====== And, Or & Invert ======]

        public IConstraint<TValueIn> And(Predicate<TValueIn> constraint, string errorMessage = null, string name = null) => And(constraint, StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));

        public IConstraint<TValueIn> And(Predicate<TValueIn> constraint, StringTemplate errorMessage, Identifier name = null) => And(new DelegateConstraint<TValueIn>(constraint).WithErrorMessage(errorMessage).WithName(name));

        public IConstraint<TValueIn> And(IConstraint<TValueIn> constraint) => new AndConstraint<TValueIn>(this, constraint);

        public IFilter<TValueIn, TResult> And<TResult>(IFilter<TValueOut, TResult> constraint) => new AndConstraint<TValueIn, TValueOut, TResult>(this, constraint);

        public IConstraintWithErrorMessage<TValueIn> Or(Predicate<TValueIn> constraint, string errorMessage = null, string name = null) => Or(constraint, StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));

        public IConstraintWithErrorMessage<TValueIn> Or(Predicate<TValueIn> constraint, StringTemplate errorMessage, Identifier name = null) => Or(new DelegateConstraint<TValueIn>(constraint).WithErrorMessage(errorMessage).WithName(name));

        public IConstraintWithErrorMessage<TValueIn> Or(IConstraint<TValueIn> constraint) => new OrConstraint<TValueIn>(this, constraint);

        public IConstraint<TValueIn> Invert() => Invert(null as StringTemplate);

        public IConstraint<TValueIn> Invert(string errorMessage, string name = null) => Invert(StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));

        public IConstraint<TValueIn> Invert(StringTemplate errorMessage, Identifier name = null) => new ConstraintInverter<TValueIn>(new ConstraintWrapper<TValueIn>(this))
            .WithErrorMessage(errorMessage)
            .WithName(name);

        #endregion

        #region [====== Conversion ======]

        IFilter<TValueIn, TValueIn> IConstraint<TValueIn>.MapInputToOutput() => new InputToOutputMapper<TValueIn>(this);

        public Predicate<TValueIn> ToDelegate() => IsSatisfiedBy;

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        public bool IsSatisfiedBy(TValueIn value)
        {
            TValueOut valueOut;

            return IsSatisfiedBy(value, out valueOut);
        }

        public bool IsSatisfiedBy(TValueIn valueIn, out TValueOut valueOut)
        {
            TValueMiddle valueMiddle;

            if (_leftConstraint.IsSatisfiedBy(valueIn, out valueMiddle))
            {
                return _rightConstraint.IsSatisfiedBy(valueMiddle, out valueOut);
            }
            valueOut = default(TValueOut);
            return false;
        }

        public bool IsNotSatisfiedBy(TValueIn value, out IErrorMessageBuilder errorMessage)
        {
            TValueOut valueOut;

            return IsNotSatisfiedBy(value, out errorMessage, out valueOut);
        }

        public bool IsNotSatisfiedBy(TValueIn valueIn, out IErrorMessageBuilder errorMessage, out TValueOut valueOut)
        {
            TValueMiddle valueMiddle;

            if (_leftConstraint.IsNotSatisfiedBy(valueIn, out errorMessage, out valueMiddle))
            {
                valueOut = default(TValueOut);
                return true;
            }
            return _rightConstraint.IsNotSatisfiedBy(valueMiddle, out errorMessage, out valueOut);
        }

        #endregion
        
    }
}
