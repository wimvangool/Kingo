
using System.Threading.Tasks;

namespace Kingo.Messaging
{    
    /// <summary>
    /// When implemented by a class, represents a unit of work that can be flushed.
    /// </summary>    
    public interface IUnitOfWork
    {        
        /// <summary>
        /// Indicates whether or not the unit of work maintains any changes that need to flushed.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current instance needs to be flushed; otherwise <c>false</c>.
        /// </returns>
        bool RequiresFlush();

        /// <summary>
        /// Flushes any pending changes to the underlying infrastructure.
        /// </summary>
        /// <exception cref="ConcurrencyException">
        /// A concurrency exception occurred.
        /// </exception>
        Task FlushAsync();
    }
}
