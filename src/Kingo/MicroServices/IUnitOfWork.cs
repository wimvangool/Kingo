using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a unit of work that manages different <see cref="IUnitOfWorkResourceManager">
    /// resource managers</see>.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Enlists the specified <paramref name="resourceManager"/> with this unit of work so that it can be flushed at the appropriate time.
        /// Note that this operation may flush the specified <paramref name="resourceManager"/> immediately.
        /// </summary>
        /// <param name="resourceManager">The resource manager to enlist.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resourceManager"/> is <c>null</c>.
        /// </exception>
        Task EnlistAsync(IUnitOfWorkResourceManager resourceManager);

        /// <summary>
        /// Flushes all pending changes by signalling to all enlisted <see cref="IUnitOfWorkResourceManager">resource managers</see>
        /// that they can flush or persist their changes.
        /// </summary>        
        Task FlushAsync();
    }
}
