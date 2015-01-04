namespace System.ComponentModel.Server
{
    /// <summary>
    /// This type is used to support implicit type conversion from a <see cref="Func{TMessageIn, TMessageOut}" /> to a
    /// <see cref="IQuery{TMessageIn, TMessageOut}" />.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
    /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
    public sealed class FuncDecorator<TMessageIn, TMessageOut> : IQuery<TMessageIn, TMessageOut>
        where TMessageIn : class, IRequestMessage<TMessageIn>
        where TMessageOut : class, IMessage<TMessageOut>
    {
        private readonly Func<TMessageIn, TMessageOut> _query;

        private FuncDecorator(Func<TMessageIn, TMessageOut> query)
        {            
            _query = query;
        }

        TMessageOut IQuery<TMessageIn, TMessageOut>.Execute(TMessageIn message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return _query.Invoke(message);
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
