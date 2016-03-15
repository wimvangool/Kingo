using System;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Kingo.Messaging;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class ChallengeRejectedEvent : DomainEvent<Guid, int>
    {        
        public ChallengeRejectedEvent(Guid challengeId, int challengeVersion)
        {
            ChallengeId = challengeId;
            ChallengeVersion = challengeVersion;
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

        protected override IValidator CreateValidator()
        {
            var validator = new ConstraintValidator<ChallengeRejectedEvent>();

            validator.VerifyThat(m => m.ChallengeId).IsNotEmpty();
            validator.VerifyThat(m => m.ChallengeVersion).IsGreaterThan(1);

            return validator;
        }
    }
}
