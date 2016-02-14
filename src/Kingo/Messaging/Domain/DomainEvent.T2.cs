using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Serves as a base class for events that are published by aggregates.
    /// </summary>
    /// <typeparam name="TKey">Key-type of the associated aggregate.</typeparam>
    /// <typeparam name="TVersion">Version-type of the associated aggregate.</typeparam>    
    [Serializable]    
    public abstract class DomainEvent<TKey, TVersion> : DomainEvent, IHasKeyAndVersion<TKey, TVersion>                
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {                
        #region [====== Key ======]

        TKey IHasKey<TKey>.Key
        {
            get { return GetKey<TKey>(); }
        }        

        #endregion

        #region [====== Version ======]

        TVersion IHasKeyAndVersion<TKey, TVersion>.Version
        {
            get { return GetVersion<TVersion>(); }
        }        

        #endregion        
    }
}
