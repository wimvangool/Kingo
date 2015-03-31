using System.ComponentModel.Server;

namespace System.ComponentModel.Client
{
    /// <summary>
    /// Represents a <see cref="IQueryDispatcher{T}" /> that delegates it's implementation to another method.
    /// </summary>
    /// <typeparam name="TMessageOut">Type of the result of this query.</typeparam>
    public class QueryDelegate<TMessageOut> : QueryDispatcher<TMessageOut> where TMessageOut : class, IMessage<TMessageOut>
    {
        private readonly IMessageProcessor _processor;
        private readonly string _messageTypeId;
        private readonly Func<TMessageOut> _method;        

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{T}" /> class.
        /// </summary>
        /// <param name="processor">The processor that is used to execute the request.</param>
        /// <param name="messageTypeId">The identifier of the message that is dispatched by this dispatcher.</param>
        /// <param name="method">The method that will be invoked by this dispatcher to execute the command.</param>         
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/> or <paramref name="method"/> is <c>null</c>.
        /// </exception> 
        public QueryDelegate(IMessageProcessor processor, string messageTypeId, Func<TMessageOut> method)
        {
            if (processor == null)
            {
                throw new ArgumentNullException("processor");
            }
            if (messageTypeId == null)
            {
                throw new ArgumentNullException("messageTypeId");
            }
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            _processor = processor;
            _messageTypeId = messageTypeId;
            _method = method;
        }

        /// <inheritdoc />
        protected override IMessageProcessor Processor
        {
            get { return _processor; }
        }

        /// <inheritdoc />
        protected override string MessageTypeId
        {
            get { return _messageTypeId; }
        }

        /// <summary>
        /// The method that is used to execute this query.
        /// </summary>
        protected Func<TMessageOut> Method
        {
            get { return _method; }
        }        

        /// <inheritdoc />
        protected override TMessageOut Execute()
        {
            return Method.Invoke();
        }        
    }
}
