
using System.Threading.Tasks;

namespace Kingo.MicroServices
{    
    /// <summary>
    /// When implemented by a class, represents a unit of work that can be flushed.
    /// </summary>    
    public interface IUnitOfWorkResourceManager
    {
        /// <summary>
        /// Identifies the resource that is managed by this resource manager, such as a database, file or blob storage.
        /// This value may be <c>null</c> if the resource is undefined.
        /// </summary>
        /// <remarks>
        /// The resource-id is used by the <see cref="IUnitOfWork"/> to determine which managers manage specific resources, so that it
        /// can optimize the flush-cycle by flushing the managers of different resources in parallel thereby optimizing I/O performance.
        /// Those resource managers that manage the same resource (such as multiple repositories using the same database), or resource
        /// managers of which the resource is undefined (resource id is <c>null</c>), will be flushed sequentially.
        /// </remarks>
        object ResourceId
        {
            get;
        }

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
        /// <exception cref="ConflictException">
        /// A concurrency exception occurred.
        /// </exception>
        Task FlushAsync();
    }
}
