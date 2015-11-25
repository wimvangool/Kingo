using System;

namespace Kingo.Constraints
{
    internal sealed class AndConstraint<TValueIn, TValueMiddle, TValueOut> : IFilter<TValueIn, TValueOut>
    {        
        private readonly IFilter<TValueIn, TValueMiddle> _leftConstraint;
        private readonly IFilter<TValueMiddle, TValueOut> _rightConstraint;

        internal AndConstraint(IFilter<TValueIn, TValueMiddle> left, IFilter<TValueMiddle, TValueOut> constraint)            
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _leftConstraint = left;
            _rightConstraint = constraint;
        }

        public void AcceptVisitor(IConstraintVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException("visitor");
            }
            visitor.VisitAnd(this, new IConstraint[] { _leftConstraint, _rightConstraint });
        }

        #region [====== And, Or & Invert ======]

        public IConstraint<TValueIn> And(Func<TValueIn, bool> constraint, string errorMessage = null, string name = null)
        {
            return And(constraint, StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));
        }

        public IConstraint<TValueIn> And(Func<TValueIn, bool> constraint, StringTemplate errorMessage, Identifier name = null)
        {
            return And(new DelegateConstraint<TValueIn>(constraint).WithErrorMessage(errorMessage).WithName(name));
        }

        public IConstraint<TValueIn> And(IConstraint<TValueIn> constraint)
        {
            return new AndConstraint<TValueIn>(this, constraint);
        }

        public IFilter<TValueIn, TResult> And<TResult>(IFilter<TValueOut, TResult> constraint)
        {
            return new AndConstraint<TValueIn, TValueOut, TResult>(this, constraint);
        }

        public IConstraintWithErrorMessage<TValueIn> Or(Func<TValueIn, bool> constraint, string errorMessage = null, string name = null)
        {
            return Or(constraint, StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));
        }

        public IConstraintWithErrorMessage<TValueIn> Or(Func<TValueIn, bool> constraint, StringTemplate errorMessage, Identifier name = null)
        {
            return Or(new DelegateConstraint<TValueIn>(constraint).WithErrorMessage(errorMessage).WithName(name));
        }

        public IConstraintWithErrorMessage<TValueIn> Or(IConstraint<TValueIn> constraint)
        {
            return new OrConstraint<TValueIn>(this, constraint);
        }

        public IConstraint<TValueIn> Invert()
        {
            return Invert(null as StringTemplate);
        }

        public IConstraint<TValueIn> Invert(string errorMessage, string name = null)
        {
            return Invert(StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));
        }

        public IConstraint<TValueIn> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<TValueIn>(new ConstraintWrapper<TValueIn>(this))
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== Conversion ======]

        IFilter<TValueIn, TValueIn> IConstraint<TValueIn>.MapInputToOutput()
        {
            return new InputToOutputMapper<TValueIn>(this);
        }
        
        public Func<TValueIn, bool> ToDelegate()
        {
            return IsSatisfiedBy;
        }

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
