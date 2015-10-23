using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Represents a wrapper for an instance implementing the <see cref="IConstraint{T}"/> interface
    /// so that it can be used as an instance implementing the <see cref="IConstraint{T, S}" /> interface.
    /// </summary>
    /// <typeparam name="TValue">Type of the constraint value.</typeparam>
    public sealed class InputToOutputMapper<TValue> : IConstraint<TValue, TValue>
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
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _constraint = constraint;
        }        

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public IConstraint<TValue> And(Func<TValue, bool> constraint, string errorMessage = null, string name = null)
        {
            return _constraint.And(constraint, errorMessage, name);
        }

        /// <inheritdoc />
        public IConstraint<TValue> And(Func<TValue, bool> constraint, StringTemplate errorMessage, Identifier name = null)
        {
            return _constraint.And(constraint, errorMessage, name);
        }

        /// <inheritdoc />
        public IConstraint<TValue> And(IConstraint<TValue> constraint)
        {
            return _constraint.And(constraint);
        }

        /// <inheritdoc />
        public IConstraint<TValue, TResult> And<TResult>(IConstraint<TValue, TResult> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return new AndConstraint<TValue, TValue, TResult>(this, constraint);
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValue> Or(Func<TValue, bool> constraint, string errorMessage = null, string name = null)
        {
            return _constraint.Or(constraint, errorMessage, name);
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValue> Or(Func<TValue, bool> constraint, StringTemplate errorMessage, Identifier name = null)
        {
            return _constraint.Or(constraint, errorMessage, name);
        }

        /// <inheritdoc />
        public IConstraintWithErrorMessage<TValue> Or(IConstraint<TValue> constraint)
        {
            return _constraint.Or(constraint);
        }

        /// <inheritdoc />
        public IConstraint<TValue> Invert()
        {
            return new InputToOutputMapper<TValue>(_constraint.Invert());
        }

        /// <inheritdoc />
        public IConstraint<TValue> Invert(string errorMessage, string name = null)
        {
            return new InputToOutputMapper<TValue>(_constraint.Invert(errorMessage, name));
        }

        /// <inheritdoc />
        public IConstraint<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new InputToOutputMapper<TValue>(_constraint.Invert(errorMessage, name));
        }

        #endregion

        #region [====== Conversion ======]

        IConstraint<TValue, TValue> IConstraint<TValue>.MapInputToOutput()
        {
            return this;
        }

        /// <inheritdoc />
        public Func<TValue, bool> ToDelegate()
        {
            return IsSatisfiedBy;
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public bool IsSatisfiedBy(TValue value)
        {
            return _constraint.IsSatisfiedBy(value);
        }

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
        public bool IsNotSatisfiedBy(TValue value, out IErrorMessage errorMessage)
        {
            return _constraint.IsNotSatisfiedBy(value, out errorMessage);
        }

        /// <inheritdoc />
        public bool IsNotSatisfiedBy(TValue valueIn, out IErrorMessage errorMessage, out TValue valueOut)
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
