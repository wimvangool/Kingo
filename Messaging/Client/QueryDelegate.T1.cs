using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a <see cref="IQueryDispatcher{T}" /> that delegates it's implementation to another method.
    /// </summary>
    /// <typeparam name="TResponse">Type of the result of this query.</typeparam>
    public class QueryDelegate<TResponse> : QueryDispatcher<TResponse> where TResponse : IMessage
    {
        private readonly Func<CancellationToken?, TResponse> _method;
        private readonly Func<Func<TResponse>, Task<TResponse>> _taskFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{T}" /> class.
        /// </summary>
        /// <param name="method">The method that is used to execute the query.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public QueryDelegate(Func<CancellationToken?, TResponse> method)
            : this(method, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{T}" /> class.
        /// </summary>
        /// <param name="method">The method that is used to execute the query.</param>
        /// <param name="taskFactory">Optional factory to create the <see cref="Task{T}" /> that will execute this query asynchronously.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public QueryDelegate(Func<CancellationToken?, TResponse> method, Func<Func<TResponse>, Task<TResponse>> taskFactory)
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
        protected Func<CancellationToken?, TResponse> Method
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
        protected override TResponse Execute(CancellationToken? token)
        {
            return Method.Invoke(token);
        }
    }
}
