namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class AndConstraint<TValueIn, TValueMiddle, TValueOut> : IConstraint<TValueIn, TValueOut>
    {
        private readonly ConstraintImplementation<TValueIn, TValueOut> _implementation;
        private readonly IConstraint<TValueIn, TValueMiddle> _leftConstraint;
        private readonly IConstraint<TValueMiddle, TValueOut> _rightConstraint;

        internal AndConstraint(IConstraint<TValueIn, TValueMiddle> leftConstraint, IConstraint<TValueMiddle, TValueOut> rightConstraint)            
        {
            _implementation = new ConstraintImplementation<TValueIn, TValueOut>(this);
            _leftConstraint = leftConstraint;
            _rightConstraint = rightConstraint;
        }

        #region [====== And, Or & Invert ======]

        public IConstraint<TValueIn> And(IConstraint<TValueIn> constraint)
        {
            return _implementation.And(constraint);
        }

        public IConstraint<TValueIn, TResult> And<TResult>(IConstraint<TValueOut, TResult> constraint)
        {
            return _implementation.And(constraint);
        }

        public IConstraintWithErrorMessage<TValueIn> Or(IConstraint<TValueIn> constraint)
        {
            return _implementation.Or(constraint);
        }

        public IConstraint<TValueIn> Invert()
        {
            return _implementation.Invert();
        }

        public IConstraint<TValueIn> Invert(string errorMessage, string name = null)
        {
            return _implementation.Invert(errorMessage, name);
        }

        public IConstraint<TValueIn> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return _implementation.Invert(errorMessage, name);
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
            return _implementation.IsSatisfiedBy(value);
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
            return _implementation.IsNotSatisfiedBy(value, out errorMessage);
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
