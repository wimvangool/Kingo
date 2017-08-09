using System;
using System.Runtime.Serialization;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class PendingChallenge
    {        
        public PendingChallenge(Guid challengeId, string playerName)
        {
            ChallengeId = challengeId;
            PlayerName = playerName;
        }

        [DataMember]
        public Guid ChallengeId
        {
            get;
            private set;
        }

        [DataMember]
        public string PlayerName
        {
            get;
            private set;
        }
    }
}
