using System;
using Kingo.BuildingBlocks.ComponentModel.Server;

namespace Kingo.BuildingBlocks.ComponentModel.Client
{
    /// <summary>
    /// Represents a <see cref="IQueryDispatcher{T}" /> that delegates it's implementation to another method.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the message that serves as the execution-parameter.</typeparam>
    /// <typeparam name="TMessageOut">Type of the result of this query.</typeparam>
    public class QueryDelegate<TMessageIn, TMessageOut> : QueryDispatcher<TMessageIn, TMessageOut>
        where TMessageIn : class, IMessage<TMessageIn>, new()
        where TMessageOut : class, IMessage<TMessageOut>
    {
        private readonly IMessageProcessor _processor;
        private readonly Func<TMessageIn, TMessageOut> _method;        

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{T}" /> class.
        /// </summary>
        /// <param name="processor">The processor that is used to execute the request.</param>
        /// <param name="method">The method that will be invoked by this dispatcher to execute the command.</param>         
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/> or <paramref name="method"/> is <c>null</c>.
        /// </exception>        
        public QueryDelegate(IMessageProcessor processor, Func<TMessageIn, TMessageOut> method) : base(new TMessageIn())
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
        /// The method that is used to execute this query.
        /// </summary>
        protected Func<TMessageIn, TMessageOut> Method
        {
            get { return _method; }
        }        

        /// <inheritdoc />
        protected override TMessageOut Execute(TMessageIn message)
        {
            return Method.Invoke(message);
        }
    }
}
