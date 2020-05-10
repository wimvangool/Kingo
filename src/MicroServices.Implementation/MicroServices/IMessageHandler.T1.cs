using System.Threading.Tasks;

namespace Kingo.MicroServices
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
        /// <exception cref="InternalOperationException">
        /// Something went wrong while processing the specified <paramref name="message"/>, such as
        /// the violation of a business rule.
        /// </exception> 
        Task HandleAsync(TMessage message, IMessageHandlerOperationContext context);
    }
}
