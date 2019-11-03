using System.Collections.Generic;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the result of handling a command or event.
    /// </summary>
    public interface IMessageHandlerOperationResult
    {
        /// <summary>
        /// Returns all messages in the order they were (scheduled to be) sent or published.
        /// </summary>
        IReadOnlyList<MessageToDispatch> Messages
        {
            get;
        }

        /// <summary>
        /// The number of message handlers that have handled the message.
        /// </summary>
        int MessageHandlerCount
        {
            get;
        }
    }
}
