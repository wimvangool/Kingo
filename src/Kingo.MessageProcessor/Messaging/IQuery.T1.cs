using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a query that accepts a message
    /// and returns a result in the form of a <typeparamref name="TMessageOut"/>.
    /// </summary>    
    /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
    public interface IQuery<TMessageOut> where TMessageOut : class, IMessage
    {
        /// <summary>
        /// Executes the query.
        /// </summary>        
        /// <returns>A <see cref="Task{T}" /> representing the operation.</returns>        
        Task<TMessageOut> ExecuteAsync();
    }
}
