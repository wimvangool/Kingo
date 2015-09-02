using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// Serves as a base class for all <see cref="IMessageHandler{TMessage}">MessageHandlers</see>.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
    /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
    public abstract class Query<TMessageIn, TMessageOut> : IQuery<TMessageIn, TMessageOut> where TMessageIn : class
    {
        /// <inheritdoc />     
        public abstract Task<TMessageOut> ExecuteAsync(TMessageIn message);
    }
}
