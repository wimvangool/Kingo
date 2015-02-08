namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as a base class for all <see cref="IMessageHandler{TMessage}">MessageHandlers</see>.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
    /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
    public abstract class Query<TMessageIn, TMessageOut> : IQuery<TMessageIn, TMessageOut> where TMessageIn : class
    {        
        TMessageOut IQuery<TMessageIn, TMessageOut>.Execute(TMessageIn message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return Execute(message);
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <returns>The result of this query.</returns>        
        protected abstract TMessageOut Execute(TMessageIn message);
    }
}
