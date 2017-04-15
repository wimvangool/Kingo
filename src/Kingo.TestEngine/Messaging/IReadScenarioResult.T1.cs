using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a result that is expected from execution of a specific query.
    /// </summary>
    /// <typeparam name="TMessageOut">Type of the result of the query.</typeparam>
    public interface IReadScenarioResult<out TMessageOut> : IScenarioResult
    {
        /// <summary>
        /// Asserts that the processor returns a specific message when executing a specific query.
        /// </summary>        
        /// <param name="assertCallback">
        /// Callback that will be used to assert the properties of the returned message.
        /// </param>
        /// <returns>A task that represents the execution of the associated scenario.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assertCallback"/> is <c>null</c>.
        /// </exception>      
        Task IsResponseAsync(Action<TMessageOut> assertCallback);
    }
}
