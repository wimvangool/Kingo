using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a controller that is able to enlist other units of work
    /// to schedule them for a flush at a later point in time.
    /// </summary>
    public interface IUnitOfWorkController : IUnitOfWork
    {               
        /// <summary>
        /// Enlists the specified <paramref name="unitOfWork"/> with the context asynchronously so that it can be flushed at the appropriate time.
        /// Note that this operation may flush the specified <paramref name="unitOfWork"/> immediately.
        /// </summary>
        /// <param name="unitOfWork">The unit of work to enlist.</param>
        /// <param name="resourceId">
        ///     Optional identifier of a resource that is used by the processor to identify which units must be flushed sequentially (those with equal identifiers)
        ///     and which can be flushed in parallel using multiple threads, thereby optimizing I/O performance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="unitOfWork"/> is <c>null</c>.
        /// </exception>
        Task EnlistAsync(IUnitOfWork unitOfWork, object resourceId = null);        
    }
}
