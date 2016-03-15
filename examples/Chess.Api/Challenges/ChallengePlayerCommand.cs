using System;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class ChallengePlayerCommand : Message
    {        
        public ChallengePlayerCommand(Guid challengeId, Guid playerId)
        {
            ChallengeId = challengeId;
            PlayerId = playerId;
        }

        [DataMember]
        public Guid ChallengeId
        {
            get;
            private set;
        }

        [DataMember]
        public Guid PlayerId
        {
            get;
            private set;
        }

        protected override IValidator CreateValidator()
        {
            var validator = new ConstraintValidator<ChallengePlayerCommand>();

            validator.VerifyThat(m => m.ChallengeId).IsNotEmpty();
            validator.VerifyThat(m => m.PlayerId).IsNotEmpty();

            return validator;
        }
    }
}
