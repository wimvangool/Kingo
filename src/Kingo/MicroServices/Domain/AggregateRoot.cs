using System;

namespace Kingo.MicroServices.Domain
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IAggregateRoot" /> interface.
    /// </summary>
    public abstract class AggregateRoot : IAggregateRoot
    {                
        internal abstract IEventBus EventBus
        {
            get;
        }

        #region [====== Modification ======]

        bool IAggregateRoot.HasBeenModified =>
            HasBeenModified;

        /// <summary>
        /// Indicates whether or not this aggregate has published any events since the last commit.
        /// </summary>
        protected abstract bool HasBeenModified
        {
            get;
        }

        private EventHandler _modified;
        
        event EventHandler IAggregateRoot.Modified
        {
            add => _modified += value;
            remove => _modified -= value;
        }

        /// <summary>
        /// This method is called when a new event has been published by this aggregate.
        /// </summary>        
        protected virtual void OnModified() =>
            _modified.Raise(this);

        #endregion

        #region [====== Removal ======]

        bool IAggregateRoot.HasBeenRemoved =>
            HasBeenRemoved;

        /// <summary>
        /// Indicates whether or not this aggregate has been removed from its repository,
        /// either in the current session or because it was soft-deleted in a previous
        /// session.
        /// </summary>
        protected bool HasBeenRemoved
        {
            get;
            set;
        }

        bool IAggregateRoot.NotifyRemoved()
        {
            try
            {
                return OnRemoved();
            }
            finally
            {
                HasBeenRemoved = true;
            }
        }

        /// <summary>
        /// This method is called when this aggregate was removed from the repository. It can be used
        /// to publish some last-minute events representing the removal of this aggregate and the end
        /// of its lifetime.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this aggregate is to be hard-deleted from the data-store;
        /// <c>false</c> if this aggregate is to be soft-deleted from the data-store.
        /// </returns>
        protected virtual bool OnRemoved() =>
            true;

        #endregion
    }
}
