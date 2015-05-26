using System.Threading.Tasks;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// When implemented by a class, represents a <see cref="IQuery{TMessageIn, TMessageOut}" /> ready to be executed.
    /// </summary>
    /// <typeparam name="TMessageOut">Type of the result of the query.</typeparam>
    public interface IQuery<TMessageOut> where TMessageOut : class, IMessage<TMessageOut>
    {
        /// <summary>
        /// Message containing the parameters of the query.
        /// </summary>
        IMessage MessageIn
        {
            get;
        }        

        /// <summary>
        /// Invokes the query and returns its result.
        /// </summary>
        /// <returns>A task that is executing the query.</returns>
        Task<TMessageOut> InvokeAsync();
    }
}
