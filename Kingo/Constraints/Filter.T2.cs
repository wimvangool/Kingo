using System;
using Kingo.Messaging;

namespace Kingo.Constraints
{
    /// <summary>
    /// Provides a base class for the <see cref="IFilter{T, S}"/> interface.
    /// </summary>
    /// <typeparam name="TValueIn">Type of the input (checked) value.</typeparam>
    /// <typeparam name="TValueOut">Type of the output value.</typeparam>
    public abstract class Filter<TValueIn, TValueOut> : Constraint, IFilterWithErrorMessage<TValueIn, TValueOut>
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="Constraint{T}" /> class.
        /// </summary>
        /// <param name="filter">Filter to copy.</param>        
        protected Filter(Filter<TValueIn, TValueOut> filter = null)
            : base(filter) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Filter{T, S}" /> class.
        /// </summary>
        /// <param name="filter">Filter to copy.</param>
        /// <param name="errorMessage">The error message of this constraint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filter"/> is <c>null</c>.
        /// </exception>
        protected Filter(Filter<TValueIn, TValueOut> filter, StringTemplate errorMessage)
            : base(filter, errorMessage) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Filter{T, S}" /> class.
        /// </summary>
        /// <param name="filter">Filter to copy.</param>
        /// <param name="name">The name of this constraint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filter"/> is <c>null</c>.
        /// </exception>
        protected Filter(Filter<TValueIn, TValueOut> filter, Identifier name)
            : base(filter, name) { }

        #region [====== Name & ErrorMessage ======]

        internal override IConstraintWithErrorMessage WithNameCore(Identifier name)
        {
            return WithName(name);
        }

        IConstraintWithErrorMessage<TValueIn> IConstraintWithErrorMessage<TValueIn>.WithName(string name)
        {
            return WithName(Identifier.ParseOrNull(name));
        }        

        IConstraintWithErrorMessage<TValueIn> IConstraintWithErrorMessage<TValueIn>.WithName(Identifier name)
        {
            return WithName(name);
        }

        IFilterWithErrorMessage<TValueIn, TValueOut> IFilterWithErrorMessage<TValueIn, TValueOut>.WithName(Identifier name)
        {
            return WithName(name);
        }

        /// <inheritdoc />
        public IFilterWithErrorMessage<TValueIn, TValueOut> WithName(string name)
        {
            return WithName(Identifier.ParseOrNull(name));
        }

        /// <summary>
        /// Creates and returns a copy of this constraint, assigning the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">New name of the constraint.</param>
        /// <returns>A copy of this constraint with the specified <paramref name="name"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is <c>null</c>.
        /// </exception>   
        public abstract IFilterWithErrorMessage<TValueIn, TValueOut> WithName(Identifier name);

        internal override IConstraintWithErrorMessage WithErrorMessageCore(StringTemplate errorMessage)
        {
            return WithErrorMessage(errorMessage);
        }

        IConstraintWithErrorMessage<TValueIn> IConstraintWithErrorMessage<TValueIn>.WithErrorMessage(string errorMessage)
        {
            return WithErrorMessage(StringTemplate.ParseOrNull(errorMessage));
        }        

        IConstraintWithErrorMessage<TValueIn> IConstraintWithErrorMessage<TValueIn>.WithErrorMessage(StringTemplate errorMessage)
        {
            return WithErrorMessage(errorMessage);
        }

        IFilterWithErrorMessage<TValueIn, TValueOut> IFilterWithErrorMessage<TValueIn, TValueOut>.WithErrorMessage(StringTemplate errorMessage)
        {
            return WithErrorMessage(errorMessage);
        }

        /// <inheritdoc />
        public IFilterWithErrorMessage<TValueIn, TValueOut> WithErrorMessage(string errorMessage)
        {
            return WithErrorMessage(StringTemplate.ParseOrNull(errorMessage));
        }

        /// <summary>
        /// Creates and returns a copy of this constraint, assigning the specified <paramref name="errorMessage"/>.
        /// </summary>
        /// <param name="errorMessage">New error message of the constraint.</param>
        /// <returns>A copy of this constraint with the specified <paramref name="errorMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        public abstract IFilterWithErrorMessage<TValueIn, TValueOut> WithErrorMessage(StringTemplate errorMessage);

        #endregion        

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public IConstraint<TValueIn> And(Func<TValueIn, bool> constraint, string errorMessage = null, string name = null)
        {
            return And(constraint, StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));
        }

        /// <inheritdoc />
        public IConstraint<TValueIn> And(Func<TValueIn, bool> constraint, StringTemplate errorMessage, Identifier name = null)
        {
            return And(new DelegateConstraint<TValueIn>(constraint).WithErrorMessage(errorMessage).WithName(name));
        }

        /// <inheritdoc />
        public virtual IConstraint<TValueIn> And(IConstraint<TValueIn> constraint)
        {
            return new AndConstraint<TValueIn>(this, constraint);
        }

        /// <inheritdoc />
        public virtual IFilter<TValueIn, TResult> And<TResult>(IFilter<TValueOut, TResult> filter)
        {
            return new AndConstraint<TValueIn, TValueOut, TResult>(this, filter);
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValueIn> Or(Func<TValueIn, bool> constraint, string errorMessage = null, string name = null)
        {
            return Or(constraint, StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValueIn> Or(Func<TValueIn, bool> constraint, StringTemplate errorMessage, Identifier name = null)
        {
            return Or(new DelegateConstraint<TValueIn>(constraint).WithErrorMessage(errorMessage).WithName(name));
        }

        /// <inheritdoc />
        public virtual IConstraintWithErrorMessage<TValueIn> Or(IConstraint<TValueIn> constraint)
        {
            return new OrConstraint<TValueIn>(this, constraint);
        }        

        IConstraint<TValueIn> IConstraint<TValueIn>.Invert()
        {
            return Invert();
        }

        IConstraint<TValueIn> IConstraint<TValueIn>.Invert(string errorMessage, string name)
        {
            return Invert(errorMessage, name);
        }

        /// <inheritdoc />
        IConstraint<TValueIn> IConstraint<TValueIn>.Invert(StringTemplate errorMessage, Identifier name)
        {
            return Invert(errorMessage, name);
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValueIn> Invert()
        {
            return Invert(null as StringTemplate);
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValueIn> Invert(string errorMessage, string name = null)
        {
            return Invert(StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name));
        }

        /// <inheritdoc />
        public virtual IConstraintWithErrorMessage<TValueIn> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<TValueIn>(this).WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== Conversion ======]
       
        IFilter<TValueIn, TValueIn> IConstraint<TValueIn>.MapInputToOutput()
        {
            return new InputToOutputMapper<TValueIn>(this);
        }

        /// <inheritdoc />
        public virtual Func<TValueIn, bool> ToDelegate()
        {
            return IsSatisfiedBy;
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public virtual bool IsSatisfiedBy(TValueIn value)
        {
            TValueOut valueOut;

            return IsSatisfiedBy(value, out valueOut);
        }

        /// <inheritdoc />
        public abstract bool IsSatisfiedBy(TValueIn valueIn, out TValueOut valueOut);

        /// <inheritdoc />
        public bool IsNotSatisfiedBy(TValueIn value, out IErrorMessageBuilder errorMessage)
        {
            TValueOut valueOut;

            return IsNotSatisfiedBy(value, out errorMessage, out valueOut);            
        }

        /// <inheritdoc />
        public virtual bool IsNotSatisfiedBy(TValueIn valueIn, out IErrorMessageBuilder errorMessage, out TValueOut valueOut)
        {
            if (IsSatisfiedBy(valueIn, out valueOut))
            {
                errorMessage = null;
                return false;
            }
            errorMessage = ErrorMessageBuilder.Build(this, valueIn);
            return true;
        }

        #endregion
    }
}
