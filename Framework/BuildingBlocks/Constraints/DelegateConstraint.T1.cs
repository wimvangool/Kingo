using System;

namespace Kingo.BuildingBlocks.Constraints
{

    #region [====== DelegateConstraints ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value satisfies a predicate that is passed in as a delegate.
    /// </summary>
    public sealed class DelegateConstraint<TValue> : Constraint<TValue>
    {
        private readonly Func<TValue, bool> _constraint;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateConstraint{T}" /> class.
        /// </summary>
        /// <param name="constraint">A delegate that represents the constraint.</param>
        /// <param name="errorMessage">The error message for this constraint.</param>
        /// <param name="name">The name of this constraint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public DelegateConstraint(Func<TValue, bool> constraint, string errorMessage = null, string name = null)
            : this(constraint, StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name)) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateConstraint{T}" /> class.
        /// </summary>
        /// <param name="constraint">A delegate that represents the constraint.</param>
        /// <param name="errorMessage">The error message for this constraint.</param>
        /// <param name="name">The name of this constraint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public DelegateConstraint(Func<TValue, bool> constraint, StringTemplate errorMessage, Identifier name)
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
            return new DelegateConstraint<TValue>(_constraint, ErrorMessage, name);
        }

        /// <inheritdoc />
        protected override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new DelegateConstraint<TValue>(_constraint, errorMessage, Name);
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return _constraint.Invoke(value);
        }
    }

    #endregion
}
