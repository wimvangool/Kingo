using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a method that is used to handle a single message.
    /// </summary>
    public abstract class HandleMessageMethod
    {
        /// <summary>
        /// Context associated with this operation.
        /// </summary>
        public abstract MicroProcessorContext Context
        {
            get;
        }

        /// <summary>
        /// Handles the message that is currently on top of the stack.
        /// </summary>        
        /// <returns>The task carrying out the operation.</returns>
        public abstract Task InvokeAsync();
    }
}
