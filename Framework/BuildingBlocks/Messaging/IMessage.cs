namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a message that can validate and copy itself.
    /// </summary>
    public interface IMessage : IValidateable
    {        
        /// <summary>
        /// Creates and returns a copy of this message.
        /// </summary>
        /// <returns>The copy of this message.</returns>
        IMessage Copy();                
    }
}
