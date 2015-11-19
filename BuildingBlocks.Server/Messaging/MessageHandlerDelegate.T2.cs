using System;
using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// This type is used to support implicit type conversion from a <see cref="Func{TResult}" /> to a
    /// <see cref="IQuery{TMessageIn, TMessageOut}" />.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
    /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
    public sealed class MessageHandlerDelegate<TMessageIn, TMessageOut> : IQuery<TMessageIn, TMessageOut> where TMessageIn : class     
    {
        private readonly Func<TMessageIn, Task<TMessageOut>> _query;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerDelegate{TMessageIn, TMessageOut}" /> class.
        /// </summary>
        /// <param name="query">The query to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public MessageHandlerDelegate(Func<TMessageIn, TMessageOut> query)
        {            
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            _query = message => Task<TMessageOut>.Factory.StartNew(() => query.Invoke(message));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerDelegate{TMessageIn, TMessageOut}" /> class.
        /// </summary>
        /// <param name="query">The query to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public MessageHandlerDelegate(Func<TMessageIn, Task<TMessageOut>> query)
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
            return _query.Invoke(message);
        }        
    }
}
