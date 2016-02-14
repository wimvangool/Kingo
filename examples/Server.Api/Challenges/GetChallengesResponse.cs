using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class GetPendingChallengesResponse : Message
    {       
        public GetPendingChallengesResponse(IEnumerable<PendingChallenge> challenges)
        {
            Challenges = challenges.ToArray();
        }

        [DataMember]
        public PendingChallenge[] Challenges
        {
            get;
            private set;
        }
    }
}
