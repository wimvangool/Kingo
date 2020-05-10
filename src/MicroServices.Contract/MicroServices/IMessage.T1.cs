using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents (the envelope of) a message, carrying its payload and metadata.
    /// </summary>
    public interface IMessage<out TContent> : IMessage
    {
        #region [====== Content ======]

        /// <summary>
        /// Returns the contents/payload of the message.
        /// </summary>
        new TContent Content
        {
            get;
        }

        #endregion
    }
}
