using System.ComponentModel.Messaging.Server;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a <see cref="ICommandDispatcher" /> that delegates it's implementation to another method.
    /// </summary>
    public class CommandDelegate<TMessage> : CommandDispatcher<TMessage> where TMessage : class, IMessage, new()
    {
        private readonly Action<TMessage, CancellationToken?> _method;
        private readonly Func<Action, Task> _taskFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDelegate{T}" /> class.
        /// </summary>        
        /// <param name="method">The method that will be invoked by this dispatcher to execute the command.</param>        
        public CommandDelegate(Action<TMessage, CancellationToken?> method)
            : this(method, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDelegate{T}" /> class.
        /// </summary>
        /// <param name="method">The method that will be invoked by this dispatcher to execute the command.</param>
        /// <param name="message">The message that serves as the execution-parameter of this command.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public CommandDelegate(Action<TMessage, CancellationToken?> method, TMessage message)
            : this(method, message, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDelegate{T}" /> class.
        /// </summary>        
        /// <param name="method">The method that will be invoked by this dispatcher to execute the command.</param>
        /// <param name="taskFactory">Optional factory to create the <see cref="Task" /> that will execute this command asynchronously.</param>        
        public CommandDelegate(Action<TMessage, CancellationToken?> method, Func<Action, Task> taskFactory)
            : this(method, null, taskFactory) { }        

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDelegate{T}" /> class.
        /// </summary>
        /// <param name="method">The method that will be invoked by this dispatcher to execute the command.</param>
        /// <param name="message">The message that serves as the execution-parameter of this command.</param>        
        /// <param name="taskFactory">Optional factory to create the <see cref="Task" /> that will execute this command asynchronously.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public CommandDelegate(Action<TMessage, CancellationToken?> method, TMessage message, Func<Action, Task> taskFactory)
            : base(message ?? new TMessage())
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
        protected Action<TMessage, CancellationToken?> Method
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
        protected override void Execute(TMessage message, CancellationToken? token)
        {
            Method.Invoke(message, token);
        }
    }
}
