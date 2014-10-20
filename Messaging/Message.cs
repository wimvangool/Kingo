namespace System.ComponentModel.Messaging
{
    /// <summary>
    /// Serves as a simple base-implementation of the <see cref="IMessage" /> interface.
    /// </summary>
    public abstract class Message : PropertyChangedBase, IMessage
    {        
        IMessage IMessage.Copy()
        {
            return Copy();
        }

        /// <summary>
        /// Creates and returns a copy of this message.
        /// </summary>
        /// <returns>A copy of this message.</returns>
        protected abstract Message Copy();
    }
}
