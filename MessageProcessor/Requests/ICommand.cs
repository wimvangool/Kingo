using System;
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
        /// <exception cref="CommandExecutionException">
        /// The command failed for (somewhat) predictable reasons, like insufficient rights, invalid parameters or
        /// because the system's state/business rules wouldn't allow this command to be executed.
        /// </exception> 
        void Execute();

        /// <summary>
        /// Executes this command asynchronously, using the specified <paramref name="dispatcher"/> to
        /// post messages back to another thread such as the UI-thread.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher that is used to post messages to another thread such as the UI-thread.
        /// </param>
        /// <returns>The task that is responsible for executing this command.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        Task ExecuteAsync(IDispatcher dispatcher);

        /// <summary>
        /// Executes this command asynchronously, using the specified <paramref name="dispatcher"/> to
        /// post messages back to another thread such as the UI-thread.
        /// </summary>
        /// <param name="dispatcher">
        /// The dispatcher that is used to post messages to another thread such as the UI-thread.
        /// </param>
        /// <param name="token">
        /// Optional token that can be used to cancel the execution of this command.
        /// </param>
        /// <returns>The task that is responsible for executing this command.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        Task ExecuteAsync(IDispatcher dispatcher, CancellationToken? token);
    }
}
