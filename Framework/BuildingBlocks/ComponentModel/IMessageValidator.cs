namespace Kingo.BuildingBlocks.ComponentModel
{
    /// <summary>
    /// When implemented by a class, represents a validator for a specific message.
    /// </summary>
    public interface IMessageValidator
    {
        /// <summary>
        /// Validates all values of a message and returns an error-tree containing all the validation-errors.
        /// </summary>                
        /// <returns>A <see cref="ValidationErrorTree" /> containing all validation-errors (if any).</returns>   
        ValidationErrorTree Validate(); 
    }
}
