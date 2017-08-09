using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Domain;
using Kingo.Messaging.Validation;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public abstract class ChallengeEvent : Event<Guid, int>
    {        
        [DataMember, AggregateId]
        public Guid ChallengeId
        {
            get;
            private set;
        }

        [DataMember, AggregateVersion]
        public int ChallengeVersion
        {
            get;
            private set;
        }        
    }
}
