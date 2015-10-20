using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IConstraint{T}"/> interface.
    /// </summary>
    /// <typeparam name="TValue">Type of the constraint value.</typeparam>
    public abstract class ConstraintWithErrorMessage<TValue> : ConstraintWithErrorMessage, IConstraintWithErrorMessage<TValue>
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintWithErrorMessage{T}" /> class.
        /// </summary>
        /// <param name="name">Name of this constraint.</param>
        /// <param name="errorMessage">Error message of this constraint.</param>        
        protected ConstraintWithErrorMessage(StringTemplate errorMessage, Identifier name)
            : base(errorMessage, name) { }

        #region [====== Name & ErrorMessage ======]

        IConstraintWithErrorMessage<TValue> IConstraintWithErrorMessage<TValue>.WithName(string name)
        {
            return WithName(Identifier.Parse(name));
        }

        IConstraintWithErrorMessage<TValue> IConstraintWithErrorMessage<TValue>.WithName(Identifier name)
        {
            return WithName(name);
        }

        internal override IConstraintWithErrorMessage WithNameCore(Identifier name)
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
        protected abstract IConstraintWithErrorMessage<TValue> WithName(Identifier name);
            
        IConstraintWithErrorMessage<TValue> IConstraintWithErrorMessage<TValue>.WithErrorMessage(string errorMessage)
        {
            return WithErrorMessage(StringTemplate.Parse(errorMessage));
        }

        IConstraintWithErrorMessage<TValue> IConstraintWithErrorMessage<TValue>.WithErrorMessage(StringTemplate errorMessage)
        {
            return WithErrorMessage(errorMessage);
        }

        internal override IConstraintWithErrorMessage WithErrorMessageCore(StringTemplate errorMessage)
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
        protected abstract IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage);

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public virtual IConstraint<TValue> And(IConstraint<TValue> constraint)
        {
            return new AndConstraint<TValue>(this, constraint);
        }

        /// <inheritdoc />
        public virtual IConstraintWithErrorMessage<TValue> Or(IConstraint<TValue> constraint)
        {
            return new OrConstraint<TValue>(this, constraint);
        }
        
        IConstraint<TValue> IConstraint<TValue>.Invert()
        {
            return Invert(null as StringTemplate);
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValue> Invert(string errorMessage, string name = null)
        {
            return Invert(StringTemplate.Parse(errorMessage), Identifier.Parse(name));
        }

        /// <inheritdoc />
        public virtual IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<TValue>(this, errorMessage, name);
        }

        #endregion

        #region [====== MapInputToOutput ======]

        /// <inheritdoc />
        public virtual IConstraint<TValue, TValue> MapInputToOutput()
        {
            return new InputToOutputMapper<TValue>(this);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public abstract bool IsSatisfiedBy(TValue value);

        /// <inheritdoc />
        public virtual bool IsNotSatisfiedBy(TValue value, out IErrorMessage errorMessage)
        {
            if (IsSatisfiedBy(value))
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
