using System.Collections.Generic;

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
        /// Validates all values of this message and returns a collection of <see cref="DataErrorInfo"/>-instances
        /// that contain error messages for each failed configuration of constraints.
        /// </summary>                
        /// <returns>
        /// A list of <see cref="DataErrorInfo" /> instances containing all validation-errors (if any). If validation
        /// succeeded, an empty list is returned.
        /// </returns>   
        IReadOnlyList<DataErrorInfo> Validate(); 
    }
}
