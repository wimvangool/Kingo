using System;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class ChallengePlayerCommand : Message<ChallengePlayerCommand>
    {
        [DataMember]
        public readonly Guid ChallengeId;

        [DataMember]
        public readonly string PlayerName;

        public ChallengePlayerCommand(Guid challengeId, string playerName)
        {
            ChallengeId = challengeId;
            PlayerName = playerName;
        }

        protected override IValidator<ChallengePlayerCommand> CreateValidator()
        {
            var validator = new ConstraintValidator<ChallengePlayerCommand>();

            validator.VerifyThat(m => m.ChallengeId).IsNotEmpty();
            validator.VerifyThat(m => m.PlayerName).IsNotNullOrEmpty().IsIdentifier();

            return validator;
        }
    }
}
