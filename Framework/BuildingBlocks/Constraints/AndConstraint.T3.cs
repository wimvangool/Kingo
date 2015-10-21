using System;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class AndConstraint<TValueIn, TValueMiddle, TValueOut> : IConstraint<TValueIn, TValueOut>
    {        
        private readonly IConstraint<TValueIn, TValueMiddle> _leftConstraint;
        private readonly IConstraint<TValueMiddle, TValueOut> _rightConstraint;

        internal AndConstraint(IConstraint<TValueIn, TValueMiddle> left, IConstraint<TValueMiddle, TValueOut> constraint)            
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _leftConstraint = left;
            _rightConstraint = constraint;
        }

        #region [====== And, Or & Invert ======]

        public IConstraint<TValueIn> And(Func<TValueIn, bool> constraint, string errorMessage = null, string name = null)
        {
            return And(constraint, StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));
        }

        public IConstraint<TValueIn> And(Func<TValueIn, bool> constraint, StringTemplate errorMessage, Identifier name = null)
        {
            return And(new DelegateConstraint<TValueIn>(constraint, errorMessage, name));
        }

        public IConstraint<TValueIn> And(IConstraint<TValueIn> constraint)
        {
            return new AndConstraint<TValueIn>(this, constraint);
        }

        public IConstraint<TValueIn, TResult> And<TResult>(IConstraint<TValueOut, TResult> constraint)
        {
            return new AndConstraint<TValueIn, TValueOut, TResult>(this, constraint);
        }

        public IConstraintWithErrorMessage<TValueIn> Or(Func<TValueIn, bool> constraint, string errorMessage = null, string name = null)
        {
            return Or(constraint, StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));
        }

        public IConstraintWithErrorMessage<TValueIn> Or(Func<TValueIn, bool> constraint, StringTemplate errorMessage, Identifier name = null)
        {
            return Or(new DelegateConstraint<TValueIn>(constraint, errorMessage, name));
        }

        public IConstraintWithErrorMessage<TValueIn> Or(IConstraint<TValueIn> constraint)
        {
            return new OrConstraint<TValueIn>(this, constraint);
        }

        public IConstraint<TValueIn> Invert()
        {
            throw new NotImplementedException();
        }        

        #endregion

        #region [====== MapInputToOutput ======]

        IConstraint<TValueIn, TValueIn> IConstraint<TValueIn>.MapInputToOutput()
        {
            return new InputToOutputMapper<TValueIn>(this);
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

        public bool IsNotSatisfiedBy(TValueIn value, out IErrorMessage errorMessage)
        {
            TValueOut valueOut;

            return IsNotSatisfiedBy(value, out errorMessage, out valueOut);
        }

        public bool IsNotSatisfiedBy(TValueIn valueIn, out IErrorMessage errorMessage, out TValueOut valueOut)
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
