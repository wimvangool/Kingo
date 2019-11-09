using System;

namespace Kingo.MicroServices.DataAnnotations
{
    /// <summary>
    /// When implemented by a class, represents a request-message wrapper that can assert whether
    /// a request message is valid or not.
    /// </summary>
    public interface IRequestMessageValidator
    {        
        /// <summary>
        /// Asserts that the message is valid.
        /// </summary>
        /// <exception cref="TestFailedException">
        /// The message is not valid.
        /// </exception>
        void IsValid();

        /// <summary>
        /// Asserts that the message is not valid and has the specified number of validation errors.
        /// </summary>
        /// <param name="expectedNumberOfErrors">
        /// The exact number of expected validation errors.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="expectedNumberOfErrors"/> is less than <c>1</c>.
        /// </exception>
        IIsNotValidResult IsNotValid(int expectedNumberOfErrors);
    }
}
