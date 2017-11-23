using System;
using Kingo.Resources;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{T}" />.
    /// </summary>
    public static class BooleanConstraints
    {
        #region [====== IsTrue ======]

        /// <summary>
        /// Verifies that the member's value is <c>true</c>.
        /// </summary>        
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, bool> IsTrue<T>(this IMemberConstraintBuilder<T, bool> member, string errorMessage = null) => member.Apply(new BooleanIsTrueConstraint().WithErrorMessage(errorMessage));

        #endregion

        #region [====== IsFalse ======]

        /// <summary>
        /// Verifies that the member's value is <c>false</c>.
        /// </summary>        
        /// <param name="member">A member.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, bool> IsFalse<T>(this IMemberConstraintBuilder<T, bool> member, string errorMessage = null) => member.Apply(new BooleanIsFalseConstraint().WithErrorMessage(errorMessage));

        #endregion
    }

    #region [====== BooleanIsTrueConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is <c>false</c>.
    /// </summary>
    public sealed class BooleanIsTrueConstraint : Constraint<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanIsTrueConstraint" /> class.
        /// </summary>        
        public BooleanIsTrueConstraint() { }

        private BooleanIsTrueConstraint(BooleanIsTrueConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private BooleanIsTrueConstraint(BooleanIsTrueConstraint constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified => StringTemplate.Parse(ErrorMessages.BooleanConstraints_IsTrue);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> WithName(Identifier name) => new BooleanIsTrueConstraint(this, name);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> WithErrorMessage(StringTemplate errorMessage) => new BooleanIsTrueConstraint(this, errorMessage);

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> Invert(StringTemplate errorMessage, Identifier name = null) => new BooleanIsFalseConstraint().WithErrorMessage(errorMessage).WithName(name);

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(bool value) => value;

        #endregion
    }

    #endregion

    #region [====== BooleanIsFalseConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a value is <c>false</c>.
    /// </summary>
    public sealed class BooleanIsFalseConstraint : Constraint<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanIsFalseConstraint" /> class.
        /// </summary>        
        public BooleanIsFalseConstraint() { }            

        private BooleanIsFalseConstraint(BooleanIsFalseConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private BooleanIsFalseConstraint(BooleanIsFalseConstraint constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified => StringTemplate.Parse(ErrorMessages.BooleanConstraints_IsFalse);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> WithName(Identifier name) => new BooleanIsFalseConstraint(this, name);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> WithErrorMessage(StringTemplate errorMessage) => new BooleanIsFalseConstraint(this, errorMessage);

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<bool> Invert(StringTemplate errorMessage, Identifier name = null) => new BooleanIsTrueConstraint().WithErrorMessage(errorMessage).WithName(name);

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(bool value) => !value;

        #endregion
    }

    #endregion
}
