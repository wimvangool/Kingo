using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Domain;
using Kingo.Messaging.Validation;

namespace Kingo.Samples.Chess.Users
{
    /// <summary>
    /// This event is raised when a new player has been registered.
    /// </summary>
    [DataContract]
    public sealed class UserRegisteredEvent : Event<Guid, int>
    {                                
        public UserRegisteredEvent(Guid userId, int version, string userName)
        {
            UserId = userId;
            UserVersion = version;
            UserName = userName;            
        }

        [DataMember, AggregateId]
        public Guid UserId
        {
            get;
            private set;
        }

        [DataMember, AggregateVersion]
        public int UserVersion
        {
            get;
            private set;
        }

        [DataMember]
        public string UserName
        {
            get;
            private set;
        }        
    }
}
