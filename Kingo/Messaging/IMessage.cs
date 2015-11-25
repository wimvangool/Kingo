using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a message that can validate and copy itself.
    /// </summary>
    public interface IMessage : IValidateable, ICloneable
    {        
        /// <summary>
        /// Creates and returns a deep copy of this message.
        /// </summary>
        /// <returns>A copy of this message.</returns>
        IMessage Copy();                
    }
}
