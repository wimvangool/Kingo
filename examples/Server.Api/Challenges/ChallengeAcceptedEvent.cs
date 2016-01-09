using System;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Kingo.Messaging;
using Kingo.Messaging.Domain;
using NServiceBus;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class ChallengeAcceptedEvent : DomainEvent<ChallengeAcceptedEvent, Guid, int>, IEvent
    {
        [DataMember]        
        public readonly Guid ChallengeId;

        [DataMember]        
        public readonly int ChallengeVersion;        

        public ChallengeAcceptedEvent(Guid challengeId, int challengeVersion)
        {
            ChallengeId = challengeId;
            ChallengeVersion = challengeVersion;            
        }

        protected override IValidator<ChallengeAcceptedEvent> CreateValidator()
        {
            var validator = new ConstraintValidator<ChallengeAcceptedEvent>();

            validator.VerifyThat(m => m.ChallengeId).IsNotEmpty();
            validator.VerifyThat(m => m.ChallengeVersion).IsGreaterThan(1);            

            return validator;
        }
    }
}
