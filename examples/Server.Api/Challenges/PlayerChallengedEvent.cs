using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class PlayerChallengedEvent : DomainEvent<Guid, int>
    {               
        public PlayerChallengedEvent(Guid challengeId, int version, Guid senderId, Guid receiverId)
        {
            ChallengeId = challengeId;
            ChallengeVersion = version;

            SenderId = senderId;
            ReceiverId = receiverId;
        }
        
        [DataMember, Key]
        public Guid ChallengeId
        {
            get;
            private set;
        }

        [DataMember, Version]
        public int ChallengeVersion
        {
            get;
            private set;
        }

        [DataMember]
        public Guid SenderId
        {
            get;
            private set;
        }

        [DataMember]
        public Guid ReceiverId
        {
            get;
            private set;
        }
    }
}
