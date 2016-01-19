using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents an entity that can be identified by it's key.
    /// </summary>
    /// <typeparam name="TKey">Type of the key of this entity.</typeparam>
    [Serializable]
    public abstract class Entity<TKey> : IHasKey<TKey>        
    {        
        #region [====== Key ======]

        TKey IHasKey<TKey>.Key
        {
            get { return Id; }
        }

        /// <summary>
        /// Identifier of this entity.
        /// </summary>
        public abstract TKey Id
        {
            get;
        }

        #endregion        
    }
}
