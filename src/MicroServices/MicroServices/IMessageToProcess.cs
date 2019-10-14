namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a message that is being handled or executed by a processor.
    /// </summary>
    public interface IMessageToProcess : IMessage
    {
        /// <summary>
        /// Indicates whether this message represents a command, event or request.
        /// </summary>
        MessageKind Kind
        {
            get;
        }
    }
}
