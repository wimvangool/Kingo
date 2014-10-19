using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a <see cref="ICommandDispatcher" /> that delegates it's implementation to another method.
    /// </summary>
    public class CommandDelegate : CommandDispatcher
    {
        private readonly Action<CancellationToken?> _method;
        private readonly Func<Action, Task> _taskFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDelegate" /> class.
        /// </summary>
        /// <param name="method">The method that will be invoked by this dispatcher to execute the command.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public CommandDelegate(Action<CancellationToken?> method)
            : this(method, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDelegate" /> class.
        /// </summary>
        /// <param name="method">The method that will be invoked by this dispatcher to execute the command.</param>
        /// <param name="taskFactory">Optional factory to create the <see cref="Task" /> that will execute this command asynchronously.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public CommandDelegate(Action<CancellationToken?> method, Func<Action, Task> taskFactory)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            _method = method;
            _taskFactory = taskFactory;
        }

        /// <summary>
        /// The method that is used to execute the command.
        /// </summary>
        protected Action<CancellationToken?> Method
        {
            get { return _method; }
        }

        /// <inheritdoc />
        protected override Task Start(Action command)
        {
            if (_taskFactory == null)
            {
                return base.Start(command);
            }
            return _taskFactory.Invoke(command);
        }

        /// <inheritdoc />
        protected override void Execute(CancellationToken? token)
        {
            Method.Invoke(token);
        }
    }
}
