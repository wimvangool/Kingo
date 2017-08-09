using System;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// When implemented by a class, represents the result of a validation test.
    /// </summary>
    public interface IValidationResult
    {
        /// <summary>
        /// Asserts that an instance error is present and, if specified, that it is equal to the specified <paramref name="expectedErrorMessage" />.
        /// </summary>
        /// <param name="expectedErrorMessage">
        /// If not <c>null</c>, specifies the expected error message.
        /// </param>
        /// <param name="comparison">
        /// Indicates how to compare the expected error message and actual error message.
        /// </param>
        /// <returns>This result.</returns>
        IValidationResult AssertInstanceError(string expectedErrorMessage = null, StringComparison comparison = StringComparison.Ordinal);

        /// <summary>
        /// Asserts that an instance error is present and invokes the specified <paramref name="assertCallback" /> which can be used
        /// to assert the contents of the error message.
        /// </summary>
        /// <param name="assertCallback">
        /// Delegate that can be used to assert the content of the error message.
        /// </param>
        /// <returns>This result.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assertCallback"/> is <c>null</c>.
        /// </exception>
        IValidationResult AssertInstanceError(Action<string> assertCallback);

        /// <summary>
        /// Asserts that an error for a member with the specified <paramref name="memberName" /> is present and, if specified,
        /// that it is equal to the specified <paramref name="expectedErrorMessage" />.
        /// </summary>
        /// <param name="memberName">Name of the member with the error message.</param>
        /// <param name="expectedErrorMessage">
        /// If not <c>null</c>, specifies the expected error message.
        /// </param>
        /// <param name="comparison">
        /// Indicates how to compare the expected error message and actual error message.
        /// </param>
        /// <returns>This result.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> is <c>null</c>.
        /// </exception>
        IValidationResult AssertMemberError(string memberName, string expectedErrorMessage = null, StringComparison comparison = StringComparison.Ordinal);

        /// <summary>
        /// Asserts that an error for a member with the specified <paramref name="memberName" /> is present and invokes the
        /// specified <paramref name="assertCallback" /> which can be used to assert the contents of the error message.
        /// </summary>
        /// <param name="memberName">Name of the member with the error message.</param>
        /// <param name="assertCallback">
        /// Delegate that can be used to assert the content of the error message.
        /// </param>
        /// <returns>This result.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> or <paramref name="assertCallback"/> is <c>null</c>.
        /// </exception>
        IValidationResult AssertMemberError(string memberName, Action<string> assertCallback);
    }
}
