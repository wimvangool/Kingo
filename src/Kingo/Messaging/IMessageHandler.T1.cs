using System.Threading.Tasks;

namespace Kingo.Messaging
{    
    /// <summary>
    /// When implemented by a class, handles messages of type <paramtyperef name="TMessage" />.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to handle.</typeparam>  
    public interface IMessageHandler<in TMessage>
    {
        /// <summary>
        /// Handles the specified <paramref name="message"/> asynchronously.
        /// </summary>
        /// <param name="message">A message.</param>        
        /// <param name="context">Context of the <see cref="IMicroProcessor" /> that is currently handling the message.</param>        
        Task HandleAsync(TMessage message, IMicroProcessorContext context);
    }
}
