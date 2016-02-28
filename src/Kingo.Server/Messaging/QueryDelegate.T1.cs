using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// This type is used to support conversion from a <see cref="Func{TResult}" /> to a <see cref="IQuery{TMessageOut}" />.
    /// </summary>    
    /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
    public sealed class QueryDelegate<TMessageOut> : IQuery<TMessageOut> where TMessageOut : class, IMessage
    {
        private readonly Func<Task<TMessageOut>> _query;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{T}" /> class.
        /// </summary>
        /// <param name="query">The query to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public QueryDelegate(Func<TMessageOut> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            _query = () => Task<TMessageOut>.Factory.StartNew(query);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{T}" /> class.
        /// </summary>
        /// <param name="query">The query to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public QueryDelegate(Func<Task<TMessageOut>> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            _query = query;
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync()
        {
            return _query.Invoke();
        }
    }
}
