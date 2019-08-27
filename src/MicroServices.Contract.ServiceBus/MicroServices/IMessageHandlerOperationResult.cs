using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the result of handling a command or event.
    /// </summary>
    public interface IMessageHandlerOperationResult
    {
        /// <summary>
        /// The events that were published during the operation.
        /// </summary>
        IReadOnlyList<IMessage> Events
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
