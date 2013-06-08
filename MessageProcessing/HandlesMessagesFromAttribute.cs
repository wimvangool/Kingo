using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// When declared on a class or method of a <see cref="IMessageHandler{T}" />, indicates
    /// of which sources the messages are accepted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HandlesMessagesFromAttribute : Attribute
    {
        private readonly MessageSources _sources;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlesMessagesFromAttribute" /> class.
        /// </summary>
        /// <param name="sources">The sources of which the messages are accepted.</param>
        public HandlesMessagesFromAttribute(MessageSources sources)
        {
            _sources = sources;
        }

        /// <summary>
        /// Returns the sources for which the messages are accepted.
        /// </summary>
        public MessageSources Sources
        {
            get { return _sources; }
        }
    }
}
