using System.ComponentModel.Messaging.Server;
using System.Threading;

namespace System.ComponentModel.Messaging.Client
{
    /// <summary>
    /// Represents a <see cref="IQueryDispatcher{T}" /> that delegates it's implementation to another method.
    /// </summary>
    /// <typeparam name="TResponse">Type of the result of this query.</typeparam>
    public class QueryDelegate<TResponse> : QueryDispatcher<TResponse> where TResponse : IMessage
    {
        private readonly Func<CancellationToken?, IProgressReporter, TResponse> _method;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{T}" /> class.
        /// </summary>
        /// <param name="method">The method that is used to execute the query.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is <c>null</c>.
        /// </exception>
        public QueryDelegate(Func<CancellationToken?, IProgressReporter, TResponse> method)
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
        protected Func<CancellationToken?, IProgressReporter, TResponse> Method
        {
            get { return _method; }
        }

        /// <inheritdoc />
        protected override TResponse Execute(CancellationToken? token, IProgressReporter reporter)
        {
            return Method.Invoke(token, reporter);
        }
    }
}
