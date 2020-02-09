
using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{    
    /// <summary>
    /// When implemented by a class, represents a component that keeps track of specific changes and can be flushed.
    /// </summary>    
    public interface IChangeTracker
    {
        /// <summary>
        /// Identifies the group this component belongs to when it comes to flushing the changes to a backing store.
        /// Components that belong to the same group are guaranteed to be flushed serially, whereas components belonging
        /// to different groups might be flushed in parallel, depending on the specified <see cref="UnitOfWorkMode" />.
        /// </summary>
        string GroupId
        {
            get;
        }

        /// <summary>
        /// Indicates whether or not this tracker manages any pending changes associated to the specified
        /// <paramref name="unitOfWorkId"/> that need to be saved.
        /// </summary>
        /// <param name="unitOfWorkId">Unique identifier of a unit of work this item has enlisted itself to.</param>
        /// <returns>
        /// <c>true</c> if this tracker manages any changes that need to be saved; otherwise <c>false</c>.
        /// </returns>
        bool HasChanges(Guid unitOfWorkId);

        /// <summary>
        /// Clears all pending changes associated to the specified <paramref name="unitOfWorkId"/> as if they were never performed.
        /// This method is called by the controller if an error occurred, giving the change-tracker a chance to clean up properly.
        /// </summary>
        /// <param name="unitOfWorkId">Unique identifier of a unit of work this item has enlisted itself to.</param>
        void UndoChanges(Guid unitOfWorkId);

        /// <summary>
        /// Saves/persists any pending changes that are associated to the specified <paramref name="unitOfWorkId"/>.
        /// </summary>
        /// <param name="unitOfWorkId">Unique identifier of a unit of work this item has enlisted itself to.</param>
        /// <exception cref="ConflictException">
        /// A concurrency exception occurred.
        /// </exception>
        Task SaveChangesAsync(Guid unitOfWorkId);
    }
}
