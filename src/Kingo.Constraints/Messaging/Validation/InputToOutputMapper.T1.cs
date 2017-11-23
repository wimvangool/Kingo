using System;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Represents a wrapper for an instance implementing the <see cref="IConstraint{T}"/> interface
    /// so that it can be used as an instance implementing the <see cref="IFilter{T, S}" /> interface.
    /// </summary>
    /// <typeparam name="TValue">Type of the constraint value.</typeparam>
    public sealed class InputToOutputMapper<TValue> : IFilter<TValue, TValue>
    {
        private readonly IConstraint<TValue> _constraint;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputToOutputMapper{T}" /> class.
        /// </summary>
        /// <param name="constraint">The constraint to wrap.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public InputToOutputMapper(IConstraint<TValue> constraint)
        {           
            _constraint = constraint ?? throw new ArgumentNullException(nameof(constraint));
        }

        /// <inheritdoc />
        public void AcceptVisitor(IConstraintVisitor visitor)
        {
            _constraint.AcceptVisitor(visitor);
        }

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public IConstraint<TValue> And(Predicate<TValue> constraint, string errorMessage = null, string name = null) =>
             _constraint.And(constraint, errorMessage, name);

        /// <inheritdoc />
        public IConstraint<TValue> And(Predicate<TValue> constraint, StringTemplate errorMessage, Identifier name = null) =>
             _constraint.And(constraint, errorMessage, name);

        /// <inheritdoc />
        public IConstraint<TValue> And(IConstraint<TValue> constraint) =>
             _constraint.And(constraint);

        /// <inheritdoc />
        public IFilter<TValue, TResult> And<TResult>(IFilter<TValue, TResult> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }
            return new AndConstraint<TValue, TValue, TResult>(this, constraint);
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValue> Or(Predicate<TValue> constraint, string errorMessage = null, string name = null) =>
             _constraint.Or(constraint, errorMessage, name);

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValue> Or(Predicate<TValue> constraint, StringTemplate errorMessage, Identifier name = null) =>
             _constraint.Or(constraint, errorMessage, name);

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValue> Or(IConstraint<TValue> constraint) =>
             _constraint.Or(constraint);

        /// <inheritdoc />
        public IConstraint<TValue> Invert() =>
             new InputToOutputMapper<TValue>(_constraint.Invert());

        /// <inheritdoc />
        public IConstraint<TValue> Invert(string errorMessage, string name = null) =>
             new InputToOutputMapper<TValue>(_constraint.Invert(errorMessage, name));

        /// <inheritdoc />
        public IConstraint<TValue> Invert(StringTemplate errorMessage, Identifier name = null) =>
             new InputToOutputMapper<TValue>(_constraint.Invert(errorMessage, name));

        #endregion

        #region [====== Conversion ======]

        IFilter<TValue, TValue> IConstraint<TValue>.MapInputToOutput() => this;

        /// <inheritdoc />
        public Predicate<TValue> ToDelegate() =>
             IsSatisfiedBy;

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public bool IsSatisfiedBy(TValue value) =>
             _constraint.IsSatisfiedBy(value);

        /// <inheritdoc />
        public bool IsSatisfiedBy(TValue valueIn, out TValue valueOut)
        {
            if (IsSatisfiedBy(valueIn))
            {
                valueOut = valueIn;
                return true;
            }
            valueOut = default(TValue);
            return false;
        }

        /// <inheritdoc />
        public bool IsNotSatisfiedBy(TValue value, out IErrorMessageBuilder errorMessage) =>
             _constraint.IsNotSatisfiedBy(value, out errorMessage);

        /// <inheritdoc />
        public bool IsNotSatisfiedBy(TValue valueIn, out IErrorMessageBuilder errorMessage, out TValue valueOut)
        {
            if (IsNotSatisfiedBy(valueIn, out errorMessage))
            {
                valueOut = default(TValue);
                return true;
            }
            valueOut = valueIn;
            return false;
        }

        #endregion        
    }
}
