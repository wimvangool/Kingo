using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IConstraint{T}"/> interface.
    /// </summary>
    /// <typeparam name="TValue">Type of the constraint value.</typeparam>
    public abstract class Constraint<TValue> : Constraint, IConstraintWithErrorMessage<TValue>
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="Constraint{T}" /> class.
        /// </summary>
        /// <param name="name">Name of this constraint.</param>
        /// <param name="errorMessage">Error message of this constraint.</param>        
        protected Constraint(StringTemplate errorMessage, Identifier name)
            : base(errorMessage, name) { }

        #region [====== Name & ErrorMessage ======]

        internal override IConstraintWithErrorMessage WithNameCore(Identifier name)
        {
            return WithName(name);
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValue> WithName(string name)
        {
            return WithName(Identifier.ParseOrNull(name));
        }

        /// <inheritdoc />
        public abstract IConstraintWithErrorMessage<TValue> WithName(Identifier name);

        internal override IConstraintWithErrorMessage WithErrorMessageCore(StringTemplate errorMessage)
        {
            return WithErrorMessage(errorMessage);
        }  

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValue> WithErrorMessage(string errorMessage)
        {
            return WithErrorMessage(StringTemplate.ParseOrNull(errorMessage));
        }

        /// <inheritdoc />
        public abstract IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage);             

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public IConstraint<TValue> And(Func<TValue, bool> constraint, string errorMessage = null, string name = null)
        {
            return And(constraint, StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));
        }

        /// <inheritdoc />
        public IConstraint<TValue> And(Func<TValue, bool> constraint, StringTemplate errorMessage, Identifier name = null)
        {
            return And(new DelegateConstraint<TValue>(constraint, errorMessage, name));
        }

        /// <inheritdoc />
        public virtual IConstraint<TValue> And(IConstraint<TValue> constraint)
        {
            return new AndConstraint<TValue>(this, constraint);
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValue> Or(Func<TValue, bool> constraint, string errorMessage = null, string name = null)
        {
            return Or(constraint, StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValue> Or(Func<TValue, bool> constraint, StringTemplate errorMessage, Identifier name = null)
        {
            return Or(new DelegateConstraint<TValue>(constraint, errorMessage, name));
        }

        /// <inheritdoc />
        public virtual IConstraintWithErrorMessage<TValue> Or(IConstraint<TValue> constraint)
        {
            return new OrConstraint<TValue>(this, constraint);
        }
        
        IConstraint<TValue> IConstraint<TValue>.Invert()
        {
            return Invert();
        }

        IConstraint<TValue> IConstraint<TValue>.Invert(string errorMessage, string name)
        {
            return Invert(errorMessage, name);
        }

        /// <inheritdoc />
        IConstraint<TValue> IConstraint<TValue>.Invert(StringTemplate errorMessage, Identifier name)
        {
            return Invert(errorMessage, name);
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValue> Invert()
        {
            return Invert(null as StringTemplate);
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValue> Invert(string errorMessage, string name = null)
        {
            return Invert(StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));
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
