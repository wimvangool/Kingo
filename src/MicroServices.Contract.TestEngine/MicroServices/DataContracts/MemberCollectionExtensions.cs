using System;

namespace Kingo.MicroServices.DataContracts
{
    /// <summary>
    /// Contains extension methods for objects that implement the <see cref="IMemberErrorCollection"/> interface.
    /// </summary>
    public static class MemberCollectionExtensions
    {
        /// <summary>
        /// Asserts that the associated <paramref name="member"/> has the specified <paramref name="expectedErrorMessage"/>.
        /// </summary>
        /// <param name="member">A member with some validation-errors.</param>
        /// <param name="expectedErrorMessage">The expected error message.</param>
        /// <param name="comparison">Indicates how the expected and actual error messages should be compared.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="expectedErrorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="comparison"/> is not a valid value.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// The specified <paramref name="expectedErrorMessage"/> was not found in the collection of error messages.
        /// </exception>
        public static void HasError(this IMemberErrorCollection member, string expectedErrorMessage, StringComparison comparison = StringComparison.Ordinal)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }
            if (expectedErrorMessage == null)
            {
                throw new ArgumentNullException(nameof(expectedErrorMessage));
            }
            member.HasError(
                actualErrorMessage => string.Compare(actualErrorMessage, expectedErrorMessage, comparison) == 0,
                ExceptionMessages.Request_ErrorMessageNotFound,
                member.MemberName, expectedErrorMessage);
        }
    }
}
