using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{String}" />.
    /// </summary>
    public static partial class StringConstraints
    {
        #region [====== DoesNotEndWith ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not end with the specified <paramref name="postfix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="postfix">The postfix this value should not end with.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> DoesNotEndWith<TMessage>(this IMemberConstraint<TMessage, string> member, string postfix, string errorMessage = null)
        {
            return member.DoesNotEndWith(postfix, StringComparison.Ordinal, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not end with the specified <paramref name="postfix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="postfix">The postfix this value should not end with.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> DoesNotEndWith<TMessage>(this IMemberConstraint<TMessage, string> member, string postfix, StringComparison compareType, string errorMessage = null)
        {
            return member.Apply(new StringEndsWithConstraint(postfix, compareType).Invert(errorMessage));
        }

        #endregion

        #region [====== EndsWith ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value ends with the specified <paramref name="postfix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="postfix">The postfix this value should end with.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> EndsWith<TMessage>(this IMemberConstraint<TMessage, string> member, string postfix, string errorMessage = null)
        {
            return member.EndsWith(postfix, StringComparison.Ordinal, errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value ends with the specified <paramref name="postfix"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="postfix">The postfix this value should end with.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> EndsWith<TMessage>(this IMemberConstraint<TMessage, string> member, string postfix, StringComparison compareType, string errorMessage = null)
        {
            return member.Apply(new StringEndsWithConstraint(postfix, compareType).WithErrorMessage(errorMessage));
        }

        #endregion
    }

    #region [====== StringEndsWithConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a <see cref="string" /> ends with a certain value.
    /// </summary>
    public sealed class StringEndsWithConstraint : Constraint<string>
    {
        private readonly string _postfix;
        private readonly StringComparison _compareType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Kingo.BuildingBlocks.Constraints.StringEndsWithConstraint" /> class.
        /// </summary>    
        /// <param name="postfix">The prefix the value should end with.</param>
        /// <param name="compareType">One of the enumeration values that specifies how the strings will be compared.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="postfix"/> is <c>null</c>.
        /// </exception>
        public StringEndsWithConstraint(string postfix, StringComparison compareType = StringComparison.Ordinal)
        {
            if (postfix == null)
            {
                throw new ArgumentNullException("postfix");
            }
            _postfix = postfix;
            _compareType = compareType;
        }

        private StringEndsWithConstraint(StringEndsWithConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            _postfix = constraint._postfix;
            _compareType = constraint._compareType;
        }

        private StringEndsWithConstraint(StringEndsWithConstraint constraint, Identifier name)
            : base(constraint, name)
        {
            _postfix = constraint._postfix;
            _compareType = constraint._compareType;
        }

        /// <summary>
        /// The postfix the value should end with.
        /// </summary>
        public string Postfix
        {
            get { return _postfix; }
        }

        /// <summary>
        /// One of the enumeration values that specifies how the strings will be compared.
        /// </summary>
        public StringComparison CompareType
        {
            get { return _compareType; }
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_EndsWith); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithName(Identifier name)
        {
            return new StringEndsWithConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringEndsWithConstraint(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<string>(this, ErrorMessages.StringConstraints_DoesNotEndWith)
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
            return value.EndsWith(_postfix, _compareType);
        }

        #endregion
    }

    #endregion
}
