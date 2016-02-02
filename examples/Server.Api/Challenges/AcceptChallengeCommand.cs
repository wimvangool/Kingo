using System;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class AcceptChallengeCommand : Message
    {
        [DataMember]
        public readonly Guid ChallengeId;

        public AcceptChallengeCommand(Guid challengeId)
        {
            ChallengeId = challengeId;
        }

        protected override IValidator CreateValidator()
        {
            var validator = new ConstraintValidator<AcceptChallengeCommand>();

            validator.VerifyThat(m => m.ChallengeId).IsNotEmpty();

            return validator;
        }
    }
}
