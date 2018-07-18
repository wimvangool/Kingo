using System.Collections.Generic;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a stack of messages as they are currently being handled by a <see cref="IMicroProcessor" />.
    /// </summary>
    public interface IMessageStackTrace : IReadOnlyList<MessageInfo>
    {
        /// <summary>
        /// Returns the source of the message that is currently on top of the stack. If <see cref="Current"/> is <c>null</c>,
        /// this property can return <see cref="MessageSources.None"/>, <see cref="MessageSources.InputStream" /> or <see cref="MessageSources.Query"/>,
        /// depending on whether the processor is doing nothing or is invoking a message handler or query that has no message-argument.
        /// </summary>
        MessageSources CurrentSource
        {
            get;
        }

        /// <summary>
        /// Returns the current message that is being handled and is on top of this stack. 
        /// </summary>
        MessageInfo Current
        {
            get;
        }
    }
}
