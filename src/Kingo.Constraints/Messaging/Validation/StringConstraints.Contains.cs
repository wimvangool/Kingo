using System;
using Kingo.Resources;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{String}" />.
    /// </summary>
    public static partial class StringConstraints
    {
        #region [====== DoesNotContain ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not contain the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> DoesNotContain<T>(this IMemberConstraintBuilder<T, string> member, char value, string errorMessage = null) =>
             member.DoesNotContain(value.ToString(), errorMessage);

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not contain the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> DoesNotContain<T>(this IMemberConstraintBuilder<T, string> member, string value, string errorMessage = null) =>
             member.Apply(new StringContainsConstraint(value).Invert(errorMessage));

        #endregion

        #region [====== Contains ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value contains the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> Contains<T>(this IMemberConstraintBuilder<T, string> member, char value, string errorMessage = null) =>
             member.Contains(value.ToString(), errorMessage);

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value contains the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageCollection" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> Contains<T>(this IMemberConstraintBuilder<T, string> member, string value, string errorMessage = null) =>
             member.Apply(new StringContainsConstraint(value).WithErrorMessage(errorMessage));

        #endregion
    }

    #region [====== StringContainsConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string contains a certain value.
    /// </summary>
    public sealed class StringContainsConstraint : Constraint<string>
    {
        /// <summary>
        /// The value to check for.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringContainsConstraint" /> class.
        /// </summary>    
        /// <param name="value">The value to check for.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        public StringContainsConstraint(string value)
        {            
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        private StringContainsConstraint(StringContainsConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            Value = constraint.Value;
        }

        private StringContainsConstraint(StringContainsConstraint constraint, Identifier name)
            : base(constraint, name)
        {
            Value = constraint.Value;
        }               

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified =>
             StringTemplate.Parse(ErrorMessages.StringConstraints_Contains);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithName(Identifier name) =>
             new StringContainsConstraint(this, name);

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithErrorMessage(StringTemplate errorMessage) =>
             new StringContainsConstraint(this, errorMessage);

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> Invert(StringTemplate errorMessage, Identifier name = null) => new ConstraintInverter<string>(this, ErrorMessages.StringConstraints_DoesNotContain)
            .WithErrorMessage(errorMessage)
            .WithName(name);

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            return value.Contains(Value);
        }

        #endregion
    }

    #endregion
}
