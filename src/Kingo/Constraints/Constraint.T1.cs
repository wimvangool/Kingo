using System;
using Kingo.Messaging;

namespace Kingo.Constraints
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
        protected Constraint() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Constraint{T}" /> class.
        /// </summary>
        /// <param name="constraint">Constraint to copy.</param>        
        protected Constraint(Constraint<TValue> constraint = null)
            : base(constraint) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Constraint{T}" /> class.
        /// </summary>
        /// <param name="constraint">Constraint to copy.</param>
        /// <param name="errorMessage">The error message of this constraint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        protected Constraint(Constraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Constraint{T}" /> class.
        /// </summary>
        /// <param name="constraint">Constraint to copy.</param>
        /// <param name="name">The name of this constraint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        protected Constraint(Constraint<TValue> constraint, Identifier name)
            : base(constraint, name) { }

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
        public virtual IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            throw NewWithNameNotSupportedException();
        }        

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
        public virtual IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            throw NewWithErrorMessageNotSupportedException();
        }        

        #endregion        

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public IConstraint<TValue> And(Predicate<TValue> constraint, string errorMessage = null, string name = null)
        {
            return And(constraint, StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));
        }

        /// <inheritdoc />
        public IConstraint<TValue> And(Predicate<TValue> constraint, StringTemplate errorMessage, Identifier name = null)
        {
            return And(new DelegateConstraint<TValue>(constraint).WithErrorMessage(errorMessage).WithName(name));
        }

        /// <inheritdoc />
        public virtual IConstraint<TValue> And(IConstraint<TValue> constraint)
        {
            return new AndConstraint<TValue>(this, constraint);
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValue> Or(Predicate<TValue> constraint, string errorMessage = null, string name = null)
        {
            return Or(constraint, StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValue> Or(Predicate<TValue> constraint, StringTemplate errorMessage, Identifier name = null)
        {
            return Or(new DelegateConstraint<TValue>(constraint).WithErrorMessage(errorMessage).WithName(name));
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
            return new ConstraintInverter<TValue>(this).WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== Conversion ======]

        /// <inheritdoc />
        public virtual IFilter<TValue, TValue> MapInputToOutput()
        {
            return new InputToOutputMapper<TValue>(this);
        }

        /// <inheritdoc />
        public virtual Predicate<TValue> ToDelegate()
        {
            return IsSatisfiedBy;
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public abstract bool IsSatisfiedBy(TValue value);

        /// <inheritdoc />
        public virtual bool IsNotSatisfiedBy(TValue value, out IErrorMessageBuilder errorMessage)
        {
            if (IsSatisfiedBy(value))
            {
                errorMessage = null;
                return false;
            }
            errorMessage = ErrorMessageBuilder.Build(this, value);
            return true;
        }

        #endregion
    }
}
