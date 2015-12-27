namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a message.
    /// </summary>    
    public interface IMessage<out TMessage> : IMessage where TMessage : class, IMessage<TMessage>
    {                
        /// <summary>
        /// Creates and returns a copy of this message.
        /// </summary>        
        /// <returns>A copy of this message.</returns>
        new TMessage Copy();        
    }
}
