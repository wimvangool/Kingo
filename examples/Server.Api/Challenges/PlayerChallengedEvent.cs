using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class PlayerChallengedEvent : DomainEvent<PlayerChallengedEvent, Guid, int>
    {
        [DataMember]
        [AggregateMember(AggregateMemberType.Key)]
        public readonly Guid ChallengeId;

        [DataMember]
        [AggregateMember(AggregateMemberType.Version)]
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
    }
}
