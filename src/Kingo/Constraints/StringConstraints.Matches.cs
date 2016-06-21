using System;
using System.Text.RegularExpressions;
using Kingo.Resources;

namespace Kingo.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{String}" />.
    /// </summary>
    public static partial class StringConstraints
    {
        #region [====== DoesNotMatch ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="pattern"/> is not a valid regular expression, or <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> DoesNotMatch<T>(this IMemberConstraintBuilder<T, string> member, string pattern, string errorMessage = null)
        {
            return member.DoesNotMatch(new Regex(pattern), errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="pattern"/> is not a valid regular expression, or <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> DoesNotMatch<T>(this IMemberConstraintBuilder<T, string> member, string pattern, RegexOptions options, string errorMessage = null)
        {
            return member.DoesNotMatch(new Regex(pattern, options), errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value does not match the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>        
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> DoesNotMatch<T>(this IMemberConstraintBuilder<T, string> member, Regex pattern, string errorMessage = null)
        {
            return member.Apply(new StringMatchesConstraint(pattern).Invert(errorMessage));
        }

        #endregion

        #region [====== Matches ======]

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="pattern"/> is not a valid regular expression, or <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> Matches<T>(this IMemberConstraintBuilder<T, string> member, string pattern, string errorMessage = null)
        {
            return member.Matches(new Regex(pattern), errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="pattern"/> is not a valid regular expression, or <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> Matches<T>(this IMemberConstraintBuilder<T, string> member, string pattern, RegexOptions options, string errorMessage = null)
        {
            return member.Matches(new Regex(pattern, options), errorMessage);
        }

        /// <summary>
        /// Verifies that the <paramref name="member" />'s value matches the specified <paramref name="pattern"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="pattern">The pattern to match.</param>       
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The specified <paramref name="member"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraintBuilder<T, string> Matches<T>(this IMemberConstraintBuilder<T, string> member, Regex pattern, string errorMessage = null)
        {
            return member.Apply(new StringMatchesConstraint(pattern).WithErrorMessage(errorMessage));
        }

        #endregion        
    }

    #region [====== StringMatchesConstraint ======]

    /// <summary>
    /// Represents a constraint that checks whether or not a string matches a specified pattern.
    /// </summary>
    public sealed class StringMatchesConstraint : Constraint<string>
    {
        /// <summary>
        /// The pattern to match.
        /// </summary>
        public readonly Regex Pattern;        

        /// <summary>
        /// Initializes a new instance of the <see cref="StringMatchesConstraint" /> class.
        /// </summary>    
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">A bitwise combination of the enumeration values that provide options for matching.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="pattern"/> is not a valid regular expression.
        /// </exception>
        public StringMatchesConstraint(string pattern, RegexOptions options = RegexOptions.None)
            : this(new Regex(pattern, options)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringMatchesConstraint" /> class.
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pattern"/> is <c>null</c>.
        /// </exception>
        public StringMatchesConstraint(Regex pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }
            Pattern = pattern;
        }

        private StringMatchesConstraint(StringMatchesConstraint constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
        {
            Pattern = constraint.Pattern;
        }

        private StringMatchesConstraint(StringMatchesConstraint constraint, Identifier name)
            : base(constraint, name)
        {
            Pattern = constraint.Pattern;
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return StringTemplate.Parse(ErrorMessages.StringConstraints_Matches); }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithName(Identifier name)
        {
            return new StringMatchesConstraint(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> WithErrorMessage(StringTemplate errorMessage)
        {
            return new StringMatchesConstraint(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<string> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return new ConstraintInverter<string>(this, ErrorMessages.StringConstraints_DoesNotMatch)
                .WithErrorMessage(errorMessage)
                .WithName(name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(string value)
        {            
            return Pattern.IsMatch(value);
        }

        #endregion
    }

    #endregion
}
