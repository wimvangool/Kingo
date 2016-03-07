using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base class for events that are published by aggregates.
    /// </summary>
    /// <typeparam name="TKey">Key-type of the associated aggregate.</typeparam>
    /// <typeparam name="TVersion">Version-type of the associated aggregate.</typeparam>    
    [Serializable]    
    public abstract class DomainEvent<TKey, TVersion> : Message, IDomainEvent<TKey, TVersion>                
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {                
        #region [====== Key ======]

        TKey IHasKey<TKey>.Key
        {
            get { return DomainEvent.GetKey<TKey>(this); }
        }

        TKey IDomainEvent<TKey, TVersion>.Key
        {
            get { return DomainEvent.GetKey<TKey>(this); }
            set { DomainEvent.SetKey(this, value); }
        }        

        #endregion

        #region [====== Version ======]        

        TVersion IHasKeyAndVersion<TKey, TVersion>.Version
        {
            get { return DomainEvent.GetVersion<TVersion>(this); }
        }

        TVersion IDomainEvent<TKey, TVersion>.Version
        {
            get { return DomainEvent.GetVersion<TVersion>(this); }
            set { DomainEvent.SetVersion(this, value); }
        }

        #endregion

        #region [====== UpgradeToLatestVersion ======]

        IDomainEvent IDomainEvent.UpgradeToLatestVersion()
        {
            return UpgradeToLatestVersion();
        }

        IDomainEvent<TKey, TVersion> IDomainEvent<TKey, TVersion>.UpgradeToLatestVersion()
        {
            return UpgradeToLatestVersion();
        }

        /// <summary>
        /// Upgrades this event to the latest version.
        /// </summary>
        /// <returns>The latest version of this event.</returns>
        protected virtual IDomainEvent<TKey, TVersion> UpgradeToLatestVersion()
        {
            return this;
        }

        #endregion
    }
}
