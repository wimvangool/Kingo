using System.Collections.Generic;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a message that can validate itself.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Creates and returns a copy of this message.
        /// </summary>
        /// <returns>The copy of this message.</returns>
        IMessage Copy();

        /// <summary>
        /// Validates all values of this message and returns whether or not any errors were found.
        /// </summary>        
        /// <param name="errorTree">
        /// If this method returns <c>true</c>, this parameter will contain all the validation-errors.
        /// </param>
        /// <returns><c>true</c> if any errors were found during validation; otherwise <c>false</c>.
        /// </returns>   
        bool TryGetValidationErrors(out ValidationErrorTree errorTree);

        /// <summary>
        /// Returns a collection of <see cref="Attribute">MessageAttributes</see> that are
        /// declared on this message and are an instance of <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attributes to select.</typeparam>        
        /// <returns>A collection of <see cref="Attribute">MessageAttributes</see>.</returns>
        IEnumerable<TAttribute> SelectAttributesOfType<TAttribute>() where TAttribute : Attribute;        
    }
}
