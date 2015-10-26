namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// When implemented by a class, represents object that can be validated.
    /// </summary>
    public interface IValidateable
    {
        /// <summary>
        /// Validates all values of this message and returns a <see cref="MessageErrorInfo"/> instance
        /// that contains error messages for all invalid members.
        /// </summary>                
        /// <returns>
        /// A <see cref="MessageErrorInfo" /> instance that contains all validation-errors (if any).
        /// </returns>   
        MessageErrorInfo Validate(); 
    }
}
