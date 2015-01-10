namespace System.ComponentModel
{
    /// <summary>
    /// Represents a message that can validate itself.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Validates all values of this message and returns whether or not any errors were found.
        /// </summary>        
        /// <param name="errorTree">
        /// If this method returns <c>true</c>, this parameter will contain all the validation-errors.
        /// </param>
        /// <returns><c>true</c> if any errors were found during validation; otherwise <c>false</c>.
        /// </returns>   
        bool TryGetValidationErrors(out ValidationErrorTree errorTree);
    }
}
