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
        /// Validates all values of this message and returns an error-tree containing all the validation-errors.
        /// </summary>                
        /// <returns>A <see cref="ValidationErrorTree" /> containing all validation-errors (if any).</returns>   
        ValidationErrorTree Validate(); 
    }
}
