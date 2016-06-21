namespace Kingo
{
    /// <summary>
    /// When implemented by a class, represent an object that can be validated.
    /// </summary>
    public interface IDataTransferObject
    {
        /// <summary>
        /// Validates this instance and returns a <see cref="ErrorInfo"/> instance
        /// that contains error messages for the instance and all invalid members.
        /// </summary>                
        /// <returns>
        /// A <see cref="ErrorInfo" /> instance that contains all validation-errors (if any).
        /// </returns>   
        ErrorInfo Validate(); 
    }
}
