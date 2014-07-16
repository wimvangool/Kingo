using System.Threading;
using System.Threading.Tasks;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents a dispatcher that dispatches or executes commands.
    /// </summary>
    public interface ICommandDispatcher : IRequestDispatcher
    {
        /// <summary>
        /// Executes the command synchronously.
        /// </summary>
        /// <exception cref="RequestExecutionException">
        /// The command failed for (somewhat) predictable reasons, like insufficient rights, invalid parameters or
        /// because the system's business logic wouldn't allow this command to be executed.
        /// </exception> 
        void Execute();

        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>        
        /// <returns>The task that is responsible for executing this command.</returns>        
        Task ExecuteAsync();

        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>        
        /// <param name="token">
        /// Optional token that can be used to cancel the execution of this command.
        /// </param>
        /// <returns>The task that is responsible for executing this command.</returns>        
        Task ExecuteAsync(CancellationToken? token);
    }
}
