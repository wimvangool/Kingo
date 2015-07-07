using System.Threading.Tasks;

namespace Syztem.ComponentModel.Server
{    
    /// <summary>
    /// When implemented by a class, handles messages of the specified <paramtyperef name="TMessage" />.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to handle.</typeparam>  
    public interface IMessageHandler<in TMessage> where TMessage : class
    {        
        /// <summary>
        /// Handles the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <returns>A task carrying out the invocation.</returns>        
        Task HandleAsync(TMessage message);
    }
}
