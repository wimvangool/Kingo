
namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a command that can be executed.
    /// </summary>
    public interface IMessageCommand
    {
        /// <summary>
        /// Returns the handler that handles the message.
        /// </summary>
        object Handler
        {
            get;
        }

        /// <summary>
        /// Returns the message to handle.
        /// </summary>
        object Message
        {
            get;
        }

        /// <summary>
        /// Executes the command by feeding the <see cref="Message"/> to the <see cref="Handler"/>.
        /// </summary>
        void Execute();
    }
}
