using System;
using Kingo.Resources;

namespace Kingo.Messaging.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{String}" />.
    /// </summary>
    public static partial class StringConstraints
    {
        #region [====== IsNotNullOrWhiteSpace ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value is not <c>null</c> or consists only of white space.
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
        public static IMemberConstraintBuilder<T, string> IsNotNullOrWhiteSpace<T>(this IMemberConstraintBuilder<T, string> member, string errorMessage = null)
        {
            return member.Apply(new StringIsNotNullOrWhiteSpaceConstraint().WithErrorMessage(errorMessage));
        }

        #endregion

        #region [====== IsNullOrWhiteSpace ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value is <c>null</c> or consists only of white space.
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
        public static IMemberConstraintBuilder<T, string> IsNullOrWhiteSpace<T>(this IMemberConstraintBuilder<T, string> member, string errorMessage = null)
        {
            return member.Apply(new StringIsNullOrWhiteSpaceConstraint().WithErrorMessage(errorMessage));
        }

        #endregion        
    }

    #region [====== StringIsNotNullOrWhiteSpaceConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string is <c>null</c> or empty.
    /// </summary>
    public sealed class StringIsNotNullOrWhiteSpaceConstraint : Constraint<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsNotNullOrWhiteSpaceConstraint" /> class.
        /// </summary>    
        public StringIsNotNullOrWhiteSpaceConstraint() { }

        private StringIsNotNullOrWhiteSpaceConstraint(StringIsNotNullOrWhiteSpaceConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private StringIsNotNullOrWhiteSpaceConstraint(StringIsNotNullOrWhiteSpaceConstraint constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_NotNullOrWhiteSpace); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithName(Identifier name)
        {
            return new StringIsNotNullOrWhiteSpaceConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsNotNullOrWhiteSpaceConstraint(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new StringIsNullOrWhiteSpaceConstraint().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        #endregion
    }

    #endregion

    #region [====== StringIsNullOrWhiteSpaceConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string is <c>null</c> or empty.
    /// </summary>
    public sealed class StringIsNullOrWhiteSpaceConstraint : Constraint<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsNullOrWhiteSpaceConstraint" /> class.
        /// </summary>    
        public StringIsNullOrWhiteSpaceConstraint() { }

        private StringIsNullOrWhiteSpaceConstraint(StringIsNullOrWhiteSpaceConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage) { }

        private StringIsNullOrWhiteSpaceConstraint(StringIsNullOrWhiteSpaceConstraint constraint, Identifier name)
            : base(constraint, name) { }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_NullOrWhiteSpace); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithName(Identifier name)
        {
            return new StringIsNullOrWhiteSpaceConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsNullOrWhiteSpaceConstraint(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new StringIsNotNullOrWhiteSpaceConstraint().WithErrorMessage(errorMessage).WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        #endregion
    }

    #endregion
}
