using System.ComponentModel.Messaging.Server;
using System.Threading;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a <see cref="IQueryDispatcher{T}" /> that delegates it's implementation to another method.
    /// </summary>
    /// <typeparam name="TRequest">Type of the message that serves as the execution-parameter.</typeparam>
    /// <typeparam name="TResponse">Type of the result of this query.</typeparam>
    public class QueryDelegate<TRequest, TResponse> : QueryDispatcher<TRequest, TResponse>
        where TRequest : class, IMessage
        where TResponse : IMessage
    {
        private readonly Func<TRequest, CancellationToken?, IProgressReporter, TResponse> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{T}" /> class.
        /// </summary>
        /// <param name="message">Message that serves as the execution-parameter of this query.</param>
        /// <param name="method">The method that is used to execute the query.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public QueryDelegate(TRequest message, Func<TRequest, CancellationToken?, IProgressReporter, TResponse> method) : base(message)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            _method = method;
        }

        /// <summary>
        /// The method that is used to execute this query.
        /// </summary>
        protected Func<TRequest, CancellationToken?, IProgressReporter, TResponse> Method
        {
            get { return _method; }
        }

        /// <inheritdoc />
        protected override TResponse Execute(TRequest message, CancellationToken? token, IProgressReporter reporter)
        {
            return Method.Invoke(message, token, reporter);
        }
    }
}
