
namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a <see cref="IMessage" /> that can make a copy of itself.
    /// </summary>
    /// <typeparam name="TMessage">Type of this message.</typeparam>
    public interface IMessage<out TMessage> : IMessage where TMessage : class, IMessage<TMessage>
    {        
        /// <summary>
        /// Creates and returns a copy of this message.
        /// </summary>
        /// <param name="makeReadOnly">Indicates whether or not the copy should be readonly.</param>
        /// <returns>
        /// A copy of this message, including the validation-state. If <paramref name="makeReadOnly"/> is <c>true</c>,
        /// all data properties of the copy will be readonly. In addition, the returned copy will be marked unchanged,
        /// even if this message is marked as changed. If the copy is readonly, the HasChanges-flag cannot be
        /// set to <c>true</c>.
        /// </returns>
        TMessage Copy(bool makeReadOnly);
    }
}
