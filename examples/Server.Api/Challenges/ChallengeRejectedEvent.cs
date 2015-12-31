using System;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Kingo.Messaging;
using Kingo.Messaging.Domain;
using NServiceBus;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class ChallengeRejectedEvent : DomainEvent<ChallengeRejectedEvent, Guid, int>, IEvent
    {
        [DataMember]
        public readonly Guid ChallengeId;

        [DataMember]
        public readonly int ChallengeVersion;

        public ChallengeRejectedEvent(Guid challengeId, int challengeVersion)
        {
            ChallengeId = challengeId;
            ChallengeVersion = challengeVersion;
        }

        protected override IValidator<ChallengeRejectedEvent> CreateValidator()
        {
            var validator = new ConstraintValidator<ChallengeRejectedEvent>();

            validator.VerifyThat(m => m.ChallengeId).IsNotEmpty();
            validator.VerifyThat(m => m.ChallengeVersion).IsGreaterThan(1);

            return validator;
        }
    }
}
