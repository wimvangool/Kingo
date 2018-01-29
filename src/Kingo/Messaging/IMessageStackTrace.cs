using System.Collections.Generic;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a stack of messages as they are currently being handled by a <see cref="IMicroProcessor" />.
    /// </summary>
    public interface IMessageStackTrace : IReadOnlyList<MessageInfo>
    {
        /// <summary>
        /// Returns the current message, or <c>null</c> if there is no current message.
        /// </summary>
        MessageInfo Current
        {
            get;
        }
    }
}
