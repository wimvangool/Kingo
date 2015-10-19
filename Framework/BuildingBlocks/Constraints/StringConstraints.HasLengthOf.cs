using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{String}" />.
    /// </summary>
    public static partial class StringConstraints
    {
        #region [====== HasLengthOf ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value length is equal to <paramref name="length"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="length">The required length of the string.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length"/> is smaller than <c>0</c>.
        /// </exception>
        public static IMemberConstraint<TMessage, string> HasLengthOf<TMessage>(this IMemberConstraint<TMessage, string> member, int length, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [====== HasLengthBetween ======]

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value length is between <paramref name="left"/> and <paramref name="right"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="left">The minimum length of the string (inclusive).</param>
        /// <param name="right">The maximum length of the string (inclusive).</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Either <paramref name="left"/> or <paramref name="right"/> is smaller than 0, or
        /// <paramref name="right"/> is smaller than <paramref name="left"/>.
        /// </exception>
        public static IMemberConstraint<TMessage, string> HasLengthBetween<TMessage>(this IMemberConstraint<TMessage, string> member, int left, int right, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the <paramref name="member"/>'s value length is in the specified <paramref name="range"/>.
        /// </summary>
        /// <param name="member">A member.</param>
        /// <param name="range">A range of allowable lengths.</param>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>The <see cref="byte"/> representation of the string.</returns>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="range" /> is <c>null</c>.
        /// </exception>        
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<TMessage, string> HasLengthBetween<TMessage>(this IMemberConstraint<TMessage, string> member, IRange<int> range, string errorMessage = null)
        {
            throw new NotImplementedException();
        }

        private static Exception NewNegativeLengthException(string paramName, int length)
        {
            var messageFormat = ExceptionMessages.StringMemberExtensions_NegativeLength;
            var message = string.Format(messageFormat, length);
            return new ArgumentOutOfRangeException(paramName, message);
        }

        #endregion
    }
}
