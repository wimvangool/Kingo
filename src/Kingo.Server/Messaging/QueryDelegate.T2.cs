using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// This type is used to support conversion from a <see cref="Func{T,TResult}" /> to a <see cref="IQuery{TMessageIn, TMessageOut}" />.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
    /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
    public sealed class QueryDelegate<TMessageIn, TMessageOut> : IQuery<TMessageIn, TMessageOut> where TMessageIn : class     
    {
        private readonly Func<TMessageIn, Task<TMessageOut>> _query;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{TMessageIn, TMessageOut}" /> class.
        /// </summary>
        /// <param name="query">The query to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public QueryDelegate(Func<TMessageIn, TMessageOut> query)
        {            
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            _query = message => Task<TMessageOut>.Factory.StartNew(() => query.Invoke(message));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryDelegate{TMessageIn, TMessageOut}" /> class.
        /// </summary>
        /// <param name="query">The query to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public QueryDelegate(Func<TMessageIn, Task<TMessageOut>> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            _query = query;
        }

        /// <inheritdoc />
        public Task<TMessageOut> ExecuteAsync(TMessageIn message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return _query.Invoke(message);
        }        
    }
}
