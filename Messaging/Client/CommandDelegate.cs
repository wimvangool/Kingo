using System.ComponentModel.Server;

namespace System.ComponentModel.Client
{
    /// <summary>
    /// Represents a <see cref="ICommandDispatcher" /> that delegates it's implementation to another method.
    /// </summary>
    public class CommandDelegate : CommandDispatcher
    {
        private readonly IMessageProcessor _processor;
        private readonly Action _method;        

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDelegate" /> class.
        /// </summary>
        /// <param name="processor">The processor that is used to execute the request.</param>
        /// <param name="method">The method that will be invoked by this dispatcher to execute the command.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/> or <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public CommandDelegate(IMessageProcessor processor, Action method)            
        {
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }            
            _processor = processor;
            _method = method;
        }

        /// <inheritdoc />
        protected override IMessageProcessor Processor
        {
            get { return _processor; }
        }

        /// <summary>
        /// The method that is used to execute the command.
        /// </summary>
        protected Action Method
        {
            get { return _method; }
        }        

        /// <inheritdoc />
        protected override void Execute()
        {
            Method.Invoke();
        }
    }
}
