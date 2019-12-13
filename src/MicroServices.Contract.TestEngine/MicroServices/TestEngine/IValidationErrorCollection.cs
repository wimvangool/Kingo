using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents a collection of validation-errors organized per member.
    /// </summary>
    public interface IValidationErrorCollection
    {
        /// <summary>
        /// Returns the error-collection of the specified member.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>All validation-errors of the specified member.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// The specified member has no errors.
        /// </exception>
        IMemberErrorCollection this[string memberName]
        {
            get;
        }
    }
}
