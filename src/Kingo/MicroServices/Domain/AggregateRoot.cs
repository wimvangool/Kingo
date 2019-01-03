using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

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

        #region [====== Modification & Removal ======]

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
        
        /// <summary>
        /// Indicates whether or not this aggregate has been removed from its repository and is therefore
        /// scheduled to be deleted.
        /// </summary>
        protected bool HasBeenRemoved
        {
            get;
            private set;
        }

        void IAggregateRoot.NotifyRemoved()
        {
            try
            {
                OnRemoved();
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
        protected virtual void OnRemoved() { }       

        #endregion
    }
}
