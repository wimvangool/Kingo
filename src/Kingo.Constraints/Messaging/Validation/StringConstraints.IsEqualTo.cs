using System;
using Kingo.Resources;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{String}" />.
    /// </summary>
    public static partial class StringConstraints
    {
        #region [====== IsNotEqualTo ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value is not equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
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
        public static IMemberConstraintBuilder<T, string> IsNotEqualTo<T>(this IMemberConstraintBuilder<T, string> member, string other, StringComparison compareType, string errorMessage = null)
        {
            return member.Apply(new StringIsNotEqualToConstraint(other, compareType).WithErrorMessage(errorMessage));
        }

        #endregion

        #region [====== IsEqualTo ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value is equal to <paramref name="other"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
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
        public static IMemberConstraintBuilder<T, string> IsEqualTo<T>(this IMemberConstraintBuilder<T, string> member, string other, StringComparison compareType, string errorMessage = null)
        {
            return member.Apply(new StringIsEqualToConstraint(other, compareType).WithErrorMessage(errorMessage));
        }

        #endregion
    }

    #region [====== StringIsNotEqualToConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string is equal to another string.
    /// </summary>
    public sealed class StringIsNotEqualToConstraint : Constraint<string>
    {
        /// <summary>
        /// The value to compare.
        /// </summary>
        public readonly string Other;

        /// <summary>
        /// One of the enumeration values that specifies how the strings will be compared.
        /// </summary>
        public readonly StringComparison CompareType;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsNotEqualToConstraint" /> class.
        /// </summary>    
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        public StringIsNotEqualToConstraint(string other, StringComparison compareType = StringComparison.Ordinal)
        {
            Other = other;
            CompareType = compareType;
        }

        private StringIsNotEqualToConstraint(StringIsNotEqualToConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            Other = constraint.Other;
            CompareType = constraint.CompareType;
        }

        private StringIsNotEqualToConstraint(StringIsNotEqualToConstraint constraint, Identifier name)
            : base(constraint, name)
        {
            Other = constraint.Other;
            CompareType = constraint.CompareType;
        }              

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.BasicConstraints_IsNotEqualTo); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithName(Identifier name)
        {
            return new StringIsNotEqualToConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsNotEqualToConstraint(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new StringIsEqualToConstraint(Other, CompareType)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(string value)
        {
            if (ReferenceEquals(value, null))
            {
                return !ReferenceEquals(Other, null);
            }
            return !value.Equals(Other, CompareType);
        }

        #endregion
    }

    #endregion

    #region [====== StringIsEqualToConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string is equal to another string.
    /// </summary>
    public sealed class StringIsEqualToConstraint : Constraint<string>
    {
        /// <summary>
        /// The value to compare.
        /// </summary>
        public readonly string Other;

        /// <summary>
        /// One of the enumeration values that specifies how the strings will be compared.
        /// </summary>
        public readonly StringComparison CompareType;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringIsEqualToConstraint" /> class.
        /// </summary>    
        /// <param name="other">The value to compare.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        public StringIsEqualToConstraint(string other, StringComparison compareType = StringComparison.Ordinal)
        {
            Other = other;
            CompareType = compareType;
        }

        private StringIsEqualToConstraint(StringIsEqualToConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            Other = constraint.Other;
            CompareType = constraint.CompareType;
        }

        private StringIsEqualToConstraint(StringIsEqualToConstraint constraint, Identifier name)
            : base(constraint, name)
        {
            Other = constraint.Other;
            CompareType = constraint.CompareType;
        }               

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.BasicConstraints_IsEqualTo); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithName(Identifier name)
        {
            return new StringIsEqualToConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringIsEqualToConstraint(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new StringIsNotEqualToConstraint(Other, CompareType)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(string value)
        {
            if (ReferenceEquals(value, null))
            {
                return ReferenceEquals(Other, null);
            }
            return value.Equals(Other, CompareType);
        }

        #endregion
    }

    #endregion
}
