using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Provides a base class for the <see cref="IConstraint{T, S}"/> interface.
    /// </summary>
    /// <typeparam name="TValueIn">Type of the input (checked) value.</typeparam>
    /// <typeparam name="TValueOut">Type of the output value.</typeparam>
    public abstract class ConstraintWithErrorMessage<TValueIn, TValueOut> : ConstraintWithErrorMessage, IConstraintWithErrorMessage<TValueIn, TValueOut>
    {
        private readonly ConstraintImplementation<TValueIn, TValueOut> _implementation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintWithErrorMessage{T}" /> class.
        /// </summary>
        /// <param name="name">Name of this constraint.</param>
        /// <param name="errorMessage">Error message of this constraint.</param>  
        protected ConstraintWithErrorMessage(StringTemplate errorMessage, Identifier name)
            : base(errorMessage, name)
        {
            _implementation = new ConstraintImplementation<TValueIn, TValueOut>(this);
        }

        #region [====== Name & ErrorMessage ======]

        IConstraintWithErrorMessage<TValueIn> IConstraintWithErrorMessage<TValueIn>.WithName(string name)
        {
            return WithName(Identifier.Parse(name));
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValueIn, TValueOut> WithName(string name)
        {
            return WithName(Identifier.Parse(name));
        }

        IConstraintWithErrorMessage<TValueIn> IConstraintWithErrorMessage<TValueIn>.WithName(Identifier name)
        {
            return WithName(name);
        }

        IConstraintWithErrorMessage<TValueIn, TValueOut> IConstraintWithErrorMessage<TValueIn, TValueOut>.WithName(Identifier name)
        {
            return WithName(name);
        }

        /// <summary>
        /// Creates and returns a copy of this constraint, assigning the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">New name of the constraint.</param>
        /// <returns>A copy of this constraint with the specified <paramref name="name"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is <c>null</c>.
        /// </exception>   
        protected abstract IConstraintWithErrorMessage<TValueIn, TValueOut> WithName(Identifier name);

        IConstraintWithErrorMessage<TValueIn> IConstraintWithErrorMessage<TValueIn>.WithErrorMessage(string errorMessage)
        {
            return WithErrorMessage(StringTemplate.Parse(errorMessage));
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValueIn, TValueOut> WithErrorMessage(string errorMessage)
        {
            return WithErrorMessage(StringTemplate.Parse(errorMessage));
        }

        IConstraintWithErrorMessage<TValueIn> IConstraintWithErrorMessage<TValueIn>.WithErrorMessage(StringTemplate errorMessage)
        {
            return WithErrorMessage(errorMessage);
        }

        IConstraintWithErrorMessage<TValueIn, TValueOut> IConstraintWithErrorMessage<TValueIn, TValueOut>.WithErrorMessage(StringTemplate errorMessage)
        {
            return WithErrorMessage(errorMessage);
        }

        /// <summary>
        /// Creates and returns a copy of this constraint, assigning the specified <paramref name="errorMessage"/>.
        /// </summary>
        /// <param name="errorMessage">New error message of the constraint.</param>
        /// <returns>A copy of this constraint with the specified <paramref name="errorMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        protected abstract IConstraintWithErrorMessage<TValueIn, TValueOut> WithErrorMessage(StringTemplate errorMessage);

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public virtual IConstraint<TValueIn> And(IConstraint<TValueIn> constraint)
        {
            return _implementation.And(constraint);
        }

        /// <inheritdoc />
        public virtual IConstraint<TValueIn, TResult> And<TResult>(IConstraint<TValueOut, TResult> constraint)
        {
            return _implementation.And(constraint);
        }

        /// <inheritdoc />
        public virtual IConstraintWithErrorMessage<TValueIn> Or(IConstraint<TValueIn> constraint)
        {
            return _implementation.Or(constraint);
        }

        /// <inheritdoc />
        public IConstraint<TValueIn> Invert()
        {
            return _implementation.Invert();
        }

        /// <inheritdoc />
        public IConstraint<TValueIn> Invert(string errorMessage, string name = null)
        {
            return _implementation.Invert(errorMessage, name);
        }

        /// <inheritdoc />
        public virtual IConstraint<TValueIn> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return _implementation.Invert(errorMessage, name);
        }

        #endregion

        #region [====== MapInputToOutput ======]
       
        IConstraint<TValueIn, TValueIn> IConstraint<TValueIn>.MapInputToOutput()
        {
            return _implementation.MapInputToOutput();
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public bool IsSatisfiedBy(TValueIn value)
        {
            return _implementation.IsSatisfiedBy(value);
        }

        /// <inheritdoc />
        public abstract bool IsSatisfiedBy(TValueIn valueIn, out TValueOut valueOut);

        /// <inheritdoc />
        public bool IsNotSatisfiedBy(TValueIn value, out IErrorMessage errorMessage)
        {
            return _implementation.IsNotSatisfiedBy(value, out errorMessage);
        }

        /// <inheritdoc />
        public virtual bool IsNotSatisfiedBy(TValueIn valueIn, out IErrorMessage errorMessage, out TValueOut valueOut)
        {
            if (IsSatisfiedBy(valueIn, out valueOut))
            {
                errorMessage = null;
                return false;
            }
            errorMessage = new FailedConstraintMessage(this);
            return true;
        }

        #endregion
    }
}
