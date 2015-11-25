using System;
using Kingo.Messaging;
using Kingo.Resources;

namespace Kingo.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{String}" />.
    /// </summary>
    public static partial class StringConstraints
    {
        #region [====== IsNotNullOrEmpty ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value is not <c>null</c> or an empty string.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> IsNotNullOrEmpty<T>(this IMemberConstraintBuilder<T, string> member, string errorMessage = null)
        {
            return member.Apply(new StringIsNotNullOrEmptyConstraint().WithErrorMessage(errorMessage));
        }

        #endregion

        #region [====== IsNullOrEmpty ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value is <c>null</c> or an empty string.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> IsNullOrEmpty<T>(this IMemberConstraintBuilder<T, string> member, string errorMessage = null)
        {
            return member.Apply(new StringIsNullOrEmptyConstraint().WithErrorMessage(errorMessage));
        }

        #endregion
    }

    #region [====== StringIsNotNullOrEmptyConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string is <c>null</c> or empty.
    /// </summary>
    public sealed class StringIsNotNullOrEmptyConstraint : Constraint<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsNotNullOrEmptyConstraint" /> class.
        /// </summary>    
        public StringIsNotNullOrEmptyConstraint() { }

        private StringIsNotNullOrEmptyConstraint(StringIsNotNullOrEmptyConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private StringIsNotNullOrEmptyConstraint(StringIsNotNullOrEmptyConstraint constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_NotNullOrEmpty); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithName(Identifier name)
        {
            return new StringIsNotNullOrEmptyConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsNotNullOrEmptyConstraint(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new StringIsNullOrEmptyConstraint().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        #endregion
    }

    #endregion

    #region [====== StringIsNullOrEmptyConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string is <c>null</c> or empty.
    /// </summary>
    public sealed class StringIsNullOrEmptyConstraint : Constraint<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsNullOrEmptyConstraint" /> class.
        /// </summary>    
        public StringIsNullOrEmptyConstraint() {}

        private StringIsNullOrEmptyConstraint(StringIsNullOrEmptyConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) {}

        private StringIsNullOrEmptyConstraint(StringIsNullOrEmptyConstraint constraint, Identifier name)
            : base(constraint, name) {}

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_NullOrEmpty); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithName(Identifier name)
        {
            return new StringIsNullOrEmptyConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsNullOrEmptyConstraint(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new StringIsNotNullOrEmptyConstraint().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(string value)
        {
            return string.IsNullOrEmpty(value);
        }

        #endregion
    }

    #endregion
}
