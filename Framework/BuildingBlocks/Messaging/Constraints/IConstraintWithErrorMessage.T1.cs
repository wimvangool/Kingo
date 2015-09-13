using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents a constraint over a certain value that is associated with a specific error message.
    /// </summary>    
    public interface IConstraintWithErrorMessage<in TMessage>
    {
        /// <summary>
        /// Returns the error message associated with this constraint.
        /// </summary>
        StringTemplate ErrorMessage
        {
            get;
        }

        /// <summary>
        /// Creates and returns the arguments of the error message based on the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The instance of the message the arguments are created from.</param>
        /// <returns>The argument(s) of the error message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        object ErrorMessageArguments(TMessage message);
    }
}
