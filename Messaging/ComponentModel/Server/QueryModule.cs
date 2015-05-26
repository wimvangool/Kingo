using System.Threading.Tasks;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as a base class for modules that are part of a <see cref="IQuery{TMessageIn, TMessageOut}" />-pipeline.
    /// </summary>
    public abstract class QueryModule
    {        
        /// <summary>
        /// Executes the specified <paramref name="query"/> while adding specific pipeline logic.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the result of <paramref name="query"/>.</typeparam>
        /// <param name="query">The handler to execute.</param>        
        /// <returns>The result of the <paramref name="query"/>.</returns>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        public abstract Task<TMessageOut> InvokeAsync<TMessageOut>(IQuery<TMessageOut> query) where TMessageOut : class, IMessage<TMessageOut>;                
    }
}
