namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a message that is being handled or executed
    /// by a <see cref="IMicroProcessor" />.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    public interface IMessageToProcess<out TMessage> : IMessageToProcess
    {
        /// <summary>
        /// Returns the message instance.
        /// </summary>
        new TMessage Content
        {
            get;
        }
    }
}
