using System.ComponentModel.Messaging.Server;
using System.Threading;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a <see cref="ICommandDispatcher" /> that delegates it's implementation to another method.
    /// </summary>
    public class CommandDelegate<TMessage> : CommandDispatcher<TMessage> where TMessage : class, IMessage
    {
        private readonly Action<TMessage, CancellationToken?, IProgressReporter> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDelegate{T}" /> class.
        /// </summary>
        /// <param name="message">The message that serves as the execution-parameter of this command.</param>
        /// <param name="method">The method that will be invoked by this dispatcher to execute the command.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public CommandDelegate(TMessage message, Action<TMessage, CancellationToken?, IProgressReporter> method) : base(message)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            _method = method;
        }

        /// <summary>
        /// The method that is used to execute the command.
        /// </summary>
        protected Action<TMessage, CancellationToken?, IProgressReporter> Method
        {
            get { return _method; }
        }

        /// <inheritdoc />
        protected override void Execute(TMessage message, CancellationToken? token, IProgressReporter reporter)
        {
            Method.Invoke(message, token, reporter);
        }
    }
}
