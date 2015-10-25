using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static class NullableConstraints
    {        
        #region [====== IsNotNull ======]

        /// <summary>
        /// Verifies whether or not the <paramref name="member"/>'s value is not <c>null</c>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>       
        /// <returns>A member containing the value of this member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, TValue> IsNotNull<TMessage, TValue>(this IMemberConstraint<TMessage, TValue?> member, string errorMessage = null) where TValue : struct
        {
            return member.Apply(new HasValueConstraint<TValue>().WithErrorMessage(errorMessage));
        }        

        #endregion
    }

    #region [====== HasValueConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a <see cref="Nullable{T}" /> has a value.
    /// </summary>
    public sealed class HasValueConstraint<TValue> : Constraint<TValue?, TValue> where TValue : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HasValueConstraint{T}" /> class.
        /// </summary>    
        public HasValueConstraint() {}

        private HasValueConstraint(HasValueConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private HasValueConstraint(HasValueConstraint<TValue> constraint, Identifier name)
            : base(constraint, name) {}

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.NullableConstraints_IsNotNull); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue?, TValue> WithName(Identifier name)
        {
            return new HasValueConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue?, TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new HasValueConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue?> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new IsNullConstraint<TValue?>().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue? value)
        {
            return value.HasValue;
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue? valueIn, out TValue valueOut)
        {
            if (valueIn.HasValue)
            {
                valueOut = valueIn.Value;
                return true;
            }
            valueOut = default(TValue);
            return false;
        }

        #endregion
    }

    #endregion
}
