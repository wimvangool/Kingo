using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Client
{
    /// <summary>
    /// Represents a dispatcher that dispatches or executes commands.
    /// </summary>
    public interface ICommandDispatcher : IRequestDispatcher
    {
        /// <summary>
        /// Executes the command synchronously.
        /// </summary>
        /// <param name="requestId">Identifier of the request.</param>            
        void Execute(Guid requestId);

        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>   
        /// <param name="requestId">Identifier of the request.</param>     
        /// <returns>The task that is responsible for executing this command.</returns>        
        Task ExecuteAsync(Guid requestId);

        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>  
        /// <param name="requestId">Identifier of the request.</param>         
        /// <param name="token">
        /// Optional token that can be used to cancel the execution of this command.
        /// </param>        
        /// <returns>The task that is responsible for executing this command.</returns>        
        Task ExecuteAsync(Guid requestId, CancellationToken token);
    }
}
