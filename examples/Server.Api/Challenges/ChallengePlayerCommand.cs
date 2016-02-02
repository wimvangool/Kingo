using System;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class ChallengePlayerCommand : Message
    {
        [DataMember]
        public readonly Guid ChallengeId;

        [DataMember]
        public readonly Guid PlayerId;

        public ChallengePlayerCommand(Guid challengeId, Guid playerId)
        {
            ChallengeId = challengeId;
            PlayerId = playerId;
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
