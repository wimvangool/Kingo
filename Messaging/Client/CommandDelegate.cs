using System.ComponentModel.Messaging.Server;
using System.Threading;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a <see cref="ICommandDispatcher" /> that delegates it's implementation to another method.
    /// </summary>
    public class CommandDelegate : CommandDispatcher
    {
        private readonly Action<CancellationToken?, IProgressReporter> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDelegate" /> class.
        /// </summary>
        /// <param name="method">The method that will be invoked by this dispatcher to execute the command.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public CommandDelegate(Action<CancellationToken?, IProgressReporter> method)
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
        protected Action<CancellationToken?, IProgressReporter> Method
        {
            get { return _method; }
        }

        /// <inheritdoc />
        protected override void Execute(CancellationToken? token, IProgressReporter reporter)
        {
            Method.Invoke(token, reporter);
        }
    }
}
