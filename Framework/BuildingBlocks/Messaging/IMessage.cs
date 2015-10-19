namespace Kingo.BuildingBlocks.Messaging
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
        /// Validates all values of this message and returns a <see cref="DataErrorInfo"/> instance
        /// that contains error messages for all invalid members.
        /// </summary>                
        /// <returns>
        /// A <see cref="DataErrorInfo" /> instance that contains all validation-errors (if any).
        /// </returns>   
        DataErrorInfo Validate(); 
    }
}
