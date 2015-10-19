using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Represents a constraint that negates another constraint.
    /// </summary>
    /// <typeparam name="TValue">Type of the constraint value.</typeparam>
    public sealed class NotConstraint<TValue> : ConstraintWithErrorMessage<TValue>
    {
        private readonly IConstraint<TValue> _constraint;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotConstraint{T}" /> class.
        /// </summary>
        /// <param name="constraint">The constraint that is to be negated.</param>
        /// <param name="errorMessage">The error message for this constraint.</param>
        /// <param name="name">The name of this constraint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public NotConstraint(IConstraint<TValue> constraint, string errorMessage = null, string name = null)
            : this(constraint, StringTemplate.Parse(errorMessage), Identifier.Parse(name)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotConstraint{T}" /> class.
        /// </summary>
        /// <param name="constraint">The constraint that is to be negated.</param>
        /// <param name="errorMessage">The error message for this constraint.</param>
        /// <param name="name">The name of this constraint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public NotConstraint(IConstraint<TValue> constraint, StringTemplate errorMessage, Identifier name)
            : base(errorMessage, name)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _constraint = constraint;
        }

        /// <inheritdoc />
        protected override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new NotConstraint<TValue>(_constraint, ErrorMessage, name);
        }

        /// <inheritdoc />
        protected override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new NotConstraint<TValue>(_constraint, errorMessage, Name);
        }        

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
    }
}
