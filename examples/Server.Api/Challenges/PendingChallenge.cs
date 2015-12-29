using System;
using System.Runtime.Serialization;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class PendingChallenge
    {
        [DataMember]
        public readonly Guid ChallengeId;        

        [DataMember]
        public readonly string PlayerName;

        public PendingChallenge(Guid challengeId, string playerName)
        {
            ChallengeId = challengeId;
            PlayerName = playerName;
        }
    }
}
