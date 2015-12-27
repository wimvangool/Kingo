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
        #region [====== DoesNotStartWith ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not start with the specified <paramref name="prefix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="prefix">The prefix this value should not start with.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> DoesNotStartWith<T>(this IMemberConstraintBuilder<T, string> member, string prefix, string errorMessage = null)
        {
            return member.DoesNotStartWith(prefix, StringComparison.Ordinal, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not start with the specified <paramref name="prefix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="prefix">The prefix this value should not start with.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> DoesNotStartWith<T>(this IMemberConstraintBuilder<T, string> member, string prefix, StringComparison compareType, string errorMessage = null)
        {
            return member.Apply(new StringStartsWithConstraint(prefix, compareType).Invert(errorMessage));
        }

        #endregion

        #region [====== StartsWith ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value starts with the specified <paramref name="prefix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="prefix">The prefix this value should start with.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> StartsWith<T>(this IMemberConstraintBuilder<T, string> member, string prefix, string errorMessage = null)
        {
            return member.StartsWith(prefix, StringComparison.Ordinal, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value starts with the specified <paramref name="prefix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="prefix">The prefix this value should start with.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> StartsWith<T>(this IMemberConstraintBuilder<T, string> member, string prefix, StringComparison compareType, string errorMessage = null)
        {
            return member.Apply(new StringStartsWithConstraint(prefix, compareType).WithErrorMessage(errorMessage));
        }

        #endregion
    }

    #region [====== StringStartsWithConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a <see cref="string" /> starts with a certain value.
    /// </summary>
    public sealed class StringStartsWithConstraint : Constraint<string>
    {
        /// <summary>
        /// The prefix the value should start with.
        /// </summary>
        public readonly string Prefix;

        /// <summary>
        /// One of the enumeration values that specifies how the strings will be compared.
        /// </summary>
        public readonly StringComparison CompareType;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringStartsWithConstraint" /> class.
        /// </summary>    
        /// <param name="prefix">The prefix the value should start with.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="prefix"/> is <c>null</c>.
        /// </exception>
        public StringStartsWithConstraint(string prefix, StringComparison compareType = StringComparison.Ordinal)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }
            Prefix = prefix;
            CompareType = compareType;
        }

        private StringStartsWithConstraint(StringStartsWithConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            Prefix = constraint.Prefix;
            CompareType = constraint.CompareType;
        }

        private StringStartsWithConstraint(StringStartsWithConstraint constraint, Identifier name)
            : base(constraint, name)
        {
            Prefix = constraint.Prefix;
            CompareType = constraint.CompareType;
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_StartsWith); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithName(Identifier name)
        {
            return new StringStartsWithConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringStartsWithConstraint(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<string>(this, ErrorMessages.StringConstraints_DoesNotStartWith)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return value.StartsWith(Prefix, CompareType);
        }

        #endregion
    }

    #endregion
}
