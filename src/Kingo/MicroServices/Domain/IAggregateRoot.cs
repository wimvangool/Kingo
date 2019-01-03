using System;
using System.Collections.Generic;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// When implemented by a class, represents the root of an aggregate.
    /// </summary>
    public interface IAggregateRoot
    {        
        #region [====== Modification & Removal ======]
        
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

        /// <summary>
        /// Notifies the aggregate that it was removed from the repository. This method can be used
        /// to publish some last minute events representing the removal of this aggregate and the end
        /// of its lifetime.
        /// </summary>
        void NotifyRemoved();

        #endregion
    }
}
