using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Domain;
using NServiceBus;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class PlayerChallengedEvent : DomainEvent, IEvent
    {
        [DataMember]        
        public readonly Guid ChallengeId;

        [DataMember]        
        public readonly int ChallengeVersion;

        [DataMember]
        public readonly Guid SenderId;

        [DataMember]
        public readonly Guid ReceiverId;

        public PlayerChallengedEvent(Guid challengeId, int version, Guid senderId, Guid receiverId)
        {
            ChallengeId = challengeId;
            ChallengeVersion = version;
            SenderId = senderId;
            ReceiverId = receiverId;
        }

        protected override Guid Key
        {
            get { return ChallengeId; }
        }

        protected override int Version
        {
            get { return ChallengeVersion; }
        }
    }
}
