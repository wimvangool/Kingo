using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Represents a constraint that is always satisfied.
    /// </summary>
    /// <typeparam name="TValue">Type of the constraint value.</typeparam>
    public sealed class NullConstraint<TValue> : ConstraintWithErrorMessage<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullConstraint{T}" /> class.
        /// </summary>
        /// <param name="errorMessage">The error message for this constraint.</param>
        /// <param name="name">The name of this constraint.</param>
        public NullConstraint(string errorMessage = null, string name = null)
            : this(StringTemplate.Parse(errorMessage), Identifier.Parse(name)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullConstraint{T}" /> class.
        /// </summary>
        /// <param name="errorMessage">The error message for this constraint.</param>
        /// <param name="name">The name of this constraint.</param>
        public NullConstraint(StringTemplate errorMessage, Identifier name)
            : base(errorMessage, name) { }

        /// <inheritdoc />
        protected override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new NullConstraint<TValue>(ErrorMessage, name);
        }

        /// <inheritdoc />
        protected override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new NullConstraint<TValue>(errorMessage, Name);
        }

        /// <inheritdoc />
        public override IConstraint<TValue> And(IConstraint<TValue> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return constraint;
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Or(IConstraint<TValue> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            return this;
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return true;
        }
    }
}
