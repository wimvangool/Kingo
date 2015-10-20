using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Represents a constraint that negates another constraint.
    /// </summary>
    /// <typeparam name="TValue">Type of the constraint value.</typeparam>
    public sealed class ConstraintInverter<TValue> : ConstraintWithErrorMessage<TValue>
    {
        private readonly IConstraintWithErrorMessage<TValue> _constraint;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintInverter{T}" /> class.
        /// </summary>
        /// <param name="constraint">The constraint that is to be negated.</param>
        /// <param name="errorMessage">The error message for this constraint.</param>
        /// <param name="name">The name of this constraint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public ConstraintInverter(IConstraintWithErrorMessage<TValue> constraint, string errorMessage = null, string name = null)
            : this(constraint, StringTemplate.Parse(errorMessage), Identifier.Parse(name)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintInverter{T}" /> class.
        /// </summary>
        /// <param name="constraint">The constraint that is to be negated.</param>
        /// <param name="errorMessage">The error message for this constraint.</param>
        /// <param name="name">The name of this constraint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public ConstraintInverter(IConstraintWithErrorMessage<TValue> constraint, StringTemplate errorMessage, Identifier name)
            : base(errorMessage, name)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _constraint = constraint;
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new ConstraintInverter<TValue>(_constraint, ErrorMessage, name);
        }

        /// <inheritdoc />
        protected override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new ConstraintInverter<TValue>(_constraint, errorMessage, Name);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return !_constraint.IsSatisfiedBy(value);
        }

        /// <inheritdoc />
        public override bool IsNotSatisfiedBy(TValue value, out IErrorMessage errorMessage)
        {
            if (_constraint.IsSatisfiedBy(value))
            {
                errorMessage = new FailedConstraintMessage(this, _constraint);
                return true;
            }
            errorMessage = null;
            return false;
        }

        #endregion
    }
}
