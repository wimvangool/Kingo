using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Serves as the base-class for all modules in a <see cref="IMessageHandler{TMessage}" /> pipeline.
    /// </summary>
    public abstract class MessageHandlerModule
    {
        /// <summary>
        /// Invokes the specified <paramref name="handler"/>.
        /// </summary>
        /// <returns>A task carrying out the invocation.</returns>        
        public abstract Task InvokeAsync(IMessageHandler handler);      
    }
}
