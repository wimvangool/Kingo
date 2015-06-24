using System.Threading.Tasks;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// This type is used to support implicit type conversion from a <see cref="Func{TMessageIn, TMessageOut}" /> to a
    /// <see cref="IQuery{TMessageIn, TMessageOut}" />.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
    /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
    public sealed class FuncDecorator<TMessageIn, TMessageOut> : IQuery<TMessageIn, TMessageOut> where TMessageIn : class     
    {
        private readonly Func<TMessageIn, TMessageOut> _query;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncDecorator{TMessageIn, TMessageOut}" /> class.
        /// </summary>
        /// <param name="query">The query to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public FuncDecorator(Func<TMessageIn, TMessageOut> query)
        {            
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            _query = query;
        }

        Task<TMessageOut> IQuery<TMessageIn, TMessageOut>.ExecuteAsync(TMessageIn message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return Task<TMessageOut>.Factory.StartNew(() => _query.Invoke(message));
        }

        /// <summary>
        /// Implicitly converts <paramref name="query"/> to an instance of <see cref="FuncDecorator{TMessageIn, TMessageOut}" />.
        /// </summary>
        /// <param name="query">The value to convert.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="query"/> is <c>null</c>;
        /// otherwise, a new instance of <see cref="FuncDecorator{TMessageIn, TMessageOut}" />.
        /// </returns>
        public static implicit operator FuncDecorator<TMessageIn, TMessageOut>(Func<TMessageIn, TMessageOut> query)
        {
            return query == null ? null : new FuncDecorator<TMessageIn, TMessageOut>(query);
        }
    }
}
