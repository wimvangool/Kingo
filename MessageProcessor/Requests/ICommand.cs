using System.Threading;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a command or write-operation.
    /// </summary>
    public interface ICommand : IRequest
    {
        /// <summary>
        /// Executes this command synchronously.
        /// </summary>
        /// <exception cref="RequestExecutionException">
        /// The command failed for (somewhat) predictable reasons, like insufficient rights, invalid parameters or
        /// because the system's state/business rules wouldn't allow this command to be executed.
        /// </exception> 
        void Execute();

        /// <summary>
        /// Executes this command asynchronously.
        /// </summary>        
        /// <returns>The task that is responsible for executing this command.</returns>        
        Task ExecuteAsync();

        /// <summary>
        /// Executes this command asynchronously.
        /// </summary>        
        /// <param name="token">
        /// Optional token that can be used to cancel the execution of this command.
        /// </param>
        /// <returns>The task that is responsible for executing this command.</returns>        
        Task ExecuteAsync(CancellationToken? token);
    }
}
