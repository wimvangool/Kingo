
namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents an internal message-bus that can be used to publish domain events and let all subscribers,
    /// direc or indirect, handle these events within the same session/transaction they were raised in.
    /// </summary> 
    public interface IDomainEventBus
    {        
        /// <summary>
        /// Publishes the specified event on this bus.
        /// </summary>
        /// <typeparam name="TMessage">Type of event to public.</typeparam>
        /// <param name="message">The event to publish.</param>                
        void Publish<TMessage>(TMessage message) where TMessage : class;
    }
}
