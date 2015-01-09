using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Client
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
        private readonly Func<TMessageIn, CancellationToken?, TMessageOut> _method;
        private readonly Func<Func<TMessageOut>, Task<TMessageOut>> _taskFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{T}" /> class.
        /// </summary>
        /// <param name="method">The method that is used to execute the query.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <c>null</c>.
        /// </exception>        
        public QueryDelegate(Func<TMessageIn, CancellationToken?, TMessageOut> method)
            : this(method, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{T}" /> class.
        /// </summary>
        /// <param name="method">The method that is used to execute the query.</param>
        /// <param name="message">Message that serves as the execution-parameter of this query.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public QueryDelegate(Func<TMessageIn, CancellationToken?, TMessageOut> method, TMessageIn message)
            : this(method, message, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{T}" /> class.
        /// </summary>
        /// <param name="method">The method that is used to execute the query.</param>
        /// <param name="taskFactory">Optional factory to create the <see cref="Task{T}" /> that will execute this query asynchronously.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public QueryDelegate(Func<TMessageIn, CancellationToken?, TMessageOut> method, Func<Func<TMessageOut>, Task<TMessageOut>> taskFactory)
            : this(method, null, taskFactory) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{T}" /> class.
        /// </summary>
        /// <param name="message">Message that serves as the execution-parameter of this query.</param>
        /// <param name="method">The method that is used to execute the query.</param>
        /// <param name="taskFactory">Optional factory to create the <see cref="Task{T}" /> that will execute this query asynchronously.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public QueryDelegate(Func<TMessageIn, CancellationToken?, TMessageOut> method, TMessageIn message, Func<Func<TMessageOut>, Task<TMessageOut>> taskFactory)
            : base(message ?? new TMessageIn())
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            _method = method;
            _taskFactory = taskFactory;
        }

        /// <summary>
        /// The method that is used to execute this query.
        /// </summary>
        protected Func<TMessageIn, CancellationToken?, TMessageOut> Method
        {
            get { return _method; }
        }

        /// <inheritdoc />
        protected override Task<TMessageOut> Start(Func<TMessageOut> query)
        {
            if (_taskFactory == null)
            {
                return base.Start(query);
            }
            return _taskFactory.Invoke(query);
        }

        /// <inheritdoc />
        protected override TMessageOut Execute(TMessageIn message, CancellationToken? token)
        {
            return Method.Invoke(message, token);
        }
    }
}
