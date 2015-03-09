using System.Collections.Generic;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a message that can validate itself.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Returns the unique identifier of this message type.
        /// </summary>
        string TypeId
        {
            get;
        }

        /// <summary>
        /// Creates and returns a copy of this message.
        /// </summary>
        /// <returns>The copy of this message.</returns>
        IMessage Copy();

        /// <summary>
        /// Validates all values of this message and returns whether or not any errors were found.
        /// </summary>
        /// <param name="exception">
        /// If this method returns <c>true</c>, this parameter will refer to a
        /// <see cref="InvalidMessageException" /> that contains all the validation-errors.
        /// </param>
        /// <returns><c>true</c> if any errors were found during validation; otherwise <c>false</c>.</returns>
        bool TryGetValidationErrors(out InvalidMessageException exception);

        /// <summary>
        /// Validates all values of this message and returns whether or not any errors were found.
        /// </summary>        
        /// <param name="errorTree">
        /// If this method returns <c>true</c>, this parameter will contain all the validation-errors.
        /// </param>
        /// <returns><c>true</c> if any errors were found during validation; otherwise <c>false</c>.</returns>   
        bool TryGetValidationErrors(out ValidationErrorTree errorTree);             
    }
}
