using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a unit of work that manages different <see cref="IChangeTracker">
    /// resource managers</see>.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Enlists the specified <paramref name="changeTracker"/> with this unit of work so that it can be flushed at the appropriate time.
        /// </summary>
        /// <param name="changeTracker">The item to enlist.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="changeTracker"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This unit of work has been disabled.
        /// </exception>
        void Enlist(IChangeTracker changeTracker);

        /// <summary>
        /// Saves/persist all pending changes by signalling to all enlisted <see cref="IChangeTracker">items</see>
        /// that they can save their changes.
        /// </summary>        
        Task SaveChangesAsync();
    }
}
