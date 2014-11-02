using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Client
{
    /// <summary>
    /// Represents a <see cref="IQueryDispatcher{T}" /> that delegates it's implementation to another method.
    /// </summary>
    /// <typeparam name="TRequest">Type of the message that serves as the execution-parameter.</typeparam>
    /// <typeparam name="TResponse">Type of the result of this query.</typeparam>
    public class QueryDelegate<TRequest, TResponse> : QueryDispatcher<TRequest, TResponse>
        where TRequest : class, IRequestMessage, new()
        where TResponse : IMessage
    {
        private readonly Func<TRequest, CancellationToken?, TResponse> _method;
        private readonly Func<Func<TResponse>, Task<TResponse>> _taskFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{T}" /> class.
        /// </summary>
        /// <param name="method">The method that is used to execute the query.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <c>null</c>.
        /// </exception>        
        public QueryDelegate(Func<TRequest, CancellationToken?, TResponse> method)
            : this(method, null, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{T}" /> class.
        /// </summary>
        /// <param name="method">The method that is used to execute the query.</param>
        /// <param name="message">Message that serves as the execution-parameter of this query.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public QueryDelegate(Func<TRequest, CancellationToken?, TResponse> method, TRequest message)
            : this(method, message, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{T}" /> class.
        /// </summary>
        /// <param name="method">The method that is used to execute the query.</param>
        /// <param name="taskFactory">Optional factory to create the <see cref="Task{T}" /> that will execute this query asynchronously.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public QueryDelegate(Func<TRequest, CancellationToken?, TResponse> method, Func<Func<TResponse>, Task<TResponse>> taskFactory)
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
        public QueryDelegate(Func<TRequest, CancellationToken?, TResponse> method, TRequest message, Func<Func<TResponse>, Task<TResponse>> taskFactory)
            : base(message ?? new TRequest())
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
        protected Func<TRequest, CancellationToken?, TResponse> Method
        {
            get { return _method; }
        }

        /// <inheritdoc />
        protected override Task<TResponse> Start(Func<TResponse> query)
        {
            if (_taskFactory == null)
            {
                return base.Start(query);
            }
            return _taskFactory.Invoke(query);
        }

        /// <inheritdoc />
        protected override TResponse Execute(TRequest message, CancellationToken? token)
        {
            return Method.Invoke(message, token);
        }
    }
}
