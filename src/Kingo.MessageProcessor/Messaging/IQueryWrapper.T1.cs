namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a <see cref="IQuery{TMessageIn, TMessageOut}" /> ready to be executed.
    /// </summary>
    /// <typeparam name="TMessageOut">Type of the result of the query.</typeparam>
    public interface IQueryWrapper<TMessageOut> : IMessageHandlerOrQueryWrapper, IQuery<TMessageOut>
        where TMessageOut : class, IMessage
    {
        /// <summary>
        /// Message containing the parameters of the query.
        /// </summary>
        IMessage MessageIn
        {
            get;
        }                
    }
}
