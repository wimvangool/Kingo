using System;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Represents a constraint that is always satisfied.
    /// </summary>
    /// <typeparam name="TValue">Type of the constraint value.</typeparam>
    public sealed class NullConstraint<TValue> : Constraint<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullConstraint{T}" /> class.
        /// </summary>        
        public NullConstraint()  { }   
     
        private NullConstraint(NullConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private NullConstraint(NullConstraint<TValue> constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name) => new NullConstraint<TValue>(this, name);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage) => new NullConstraint<TValue>(this, errorMessage);

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraint<TValue> And(IConstraint<TValue> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }
            return constraint;
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Or(IConstraint<TValue> constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }
            return this;
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value) => true;

        #endregion
    }
}
