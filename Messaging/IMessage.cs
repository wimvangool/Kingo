namespace System.ComponentModel.Messaging
{
    /// <summary>
    /// Represents a message.
    /// </summary>    
    public interface IMessage
    {
        /// <summary>
        /// Creates and returns a copy of this message.
        /// </summary>        
        /// <returns>A copy of this message.</returns>
        IMessage Copy();        
    }
}
