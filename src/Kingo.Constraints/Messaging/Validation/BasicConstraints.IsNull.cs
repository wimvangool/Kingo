using System;
using Kingo.Resources;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{T}" />.
    /// </summary>
    public static partial class BasicConstraints
    {
        #region [====== IsNotNull ======]

        /// <summary>
        /// Verifies whether or not the member's value is not <c>null</c>.
        /// </summary>     
        /// <param name="member">A member.</param>   
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>A member that has been merged with the specified member.</returns>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception> 
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, TValue> IsNotNull<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, string errorMessage = null) => member.Apply(new IsNotNullConstraint<TValue>().WithErrorMessage(errorMessage));

        #endregion

        #region [====== IsNull ======]

        /// <summary>
        /// Verifies whether or not the member's value is <c>null</c>.
        /// </summary> 
        /// <param name="member">A member.</param> 
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>        
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, TValue> IsNull<T, TValue>(this IMemberConstraintBuilder<T, TValue> member, string errorMessage = null) => member.Apply(new IsNullConstraint<TValue>().WithErrorMessage(errorMessage));

        #endregion        
    }

    #region [====== IsNotNullConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is  not <c>null</c>.
    /// </summary>
    public sealed class IsNotNullConstraint<TValue> : Constraint<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsNotNullConstraint{T}" /> class.
        /// </summary>        
        public IsNotNullConstraint() { }            
        
        private IsNotNullConstraint(IsNotNullConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private IsNotNullConstraint(IsNotNullConstraint<TValue> constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified => StringTemplate.Parse(ErrorMessages.BasicConstraints_IsNotNull);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name) => new IsNotNullConstraint<TValue>(this, name);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage) => new IsNotNullConstraint<TValue>(this, errorMessage);

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null) => new IsNullConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value) => !ReferenceEquals(value, null);

        #endregion
    }

    #endregion

    #region [====== IsNullConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is <c>null</c>.
    /// </summary>
    public sealed class IsNullConstraint<TValue> : Constraint<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsNullConstraint{T}" /> class.
        /// </summary>       
        public IsNullConstraint() { }

        private IsNullConstraint(IsNullConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private IsNullConstraint(IsNullConstraint<TValue> constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified => StringTemplate.Parse(ErrorMessages.BasicConstraints_IsNull);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name) => new IsNullConstraint<TValue>(this, name);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage) => new IsNullConstraint<TValue>(this, errorMessage);

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null) => new IsNotNullConstraint<TValue>().WithErrorMessage(errorMessage).WithName(name);

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value) => ReferenceEquals(value, null);

        #endregion
    }

    #endregion
}
