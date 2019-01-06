using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, represents the root of an aggregate.
    /// </summary>
    public interface IAggregateRoot
    {        
        #region [====== Modification ======]
        
        /// <summary>
        /// Indicates whether or not this aggregate has published new events since the last commit.
        /// </summary>
        bool HasBeenModified
        {
            get;
        }

        /// <summary>
        /// This event is raised when a new event has been published by this aggregate.
        /// </summary>
        event EventHandler Modified;

        #endregion

        #region [====== Removal ======]

        /// <summary>
        /// Indicates whether or not this aggregate has been removed from the repository.
        /// </summary>
        bool HasBeenRemoved
        {
            get;
        }

        /// <summary>
        /// Notifies the aggregate that it was removed from the repository. This method can be used
        /// to publish some last minute events representing the removal of this aggregate and the end
        /// of its lifetime.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this aggregate should be hard-deleted from the data-store;
        /// <c>false</c> if this aggregate is only soft-deleted.
        /// </returns>
        bool NotifyRemoved();

        #endregion
    }
}
