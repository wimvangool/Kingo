using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Kingo.Messaging.Validation;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class GetPendingChallengesResponse
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
