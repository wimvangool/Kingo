using System.Threading;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a task that can executes a command asynchronously.
    /// </summary>
    public class CommandExecutionTask : AsyncExecutionTask<ICommandDispatcher>
    {
        private readonly ICommandDispatcher _dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionTask" /> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher that is used to execute the command.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <c>null</c>.
        /// </exception>
        public CommandExecutionTask(ICommandDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            _dispatcher = dispatcher;
        }

        /// <inheritdoc />
        protected override ICommandDispatcher Dispatcher
        {
            get { return _dispatcher; }
        }

        /// <inheritdoc />
        protected override void Execute(CancellationToken token)
        {
            _dispatcher.ExecuteAsync(RequestId, token);
        }
    }
}
