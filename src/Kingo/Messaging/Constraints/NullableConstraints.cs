using System;
using Kingo.Resources;

namespace Kingo.Messaging.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{T}" />.
    /// </summary>
    public static class NullableConstraints
    {        
        #region [====== HasValue ======]

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
        public static IMemberConstraintBuilder<T, TValue> HasValue<T, TValue>(this IMemberConstraintBuilder<T, TValue?> member, string errorMessage = null) where TValue : struct
        {
            return member.Apply(new HasValueFilter<TValue>().WithErrorMessage(errorMessage));
        }        

        #endregion
    }

    #region [====== HasValueFilter ======]

    /// <summary>
    /// Represents a filter that transforms a <see cref="Nullable{T}" /> into a value.
    /// </summary>
    public sealed class HasValueFilter<TValue> : Filter<TValue?, TValue> where TValue : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HasValueFilter{T}" /> class.
        /// </summary>    
        public HasValueFilter() {}

        private HasValueFilter(HasValueFilter<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private HasValueFilter(HasValueFilter<TValue> constraint, Identifier name)
            : base(constraint, name) {}

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.NullableConstraints_HasValue); }
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<TValue?, TValue> WithName(Identifier name)
        {
            return new HasValueFilter<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IFilterWithErrorMessage<TValue?, TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new HasValueFilter<TValue>(this, errorMessage);
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
