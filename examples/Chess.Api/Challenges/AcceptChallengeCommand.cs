using System;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class AcceptChallengeCommand : Message
    {
        public AcceptChallengeCommand(Guid challengeId)
        {
            ChallengeId = challengeId;
        }

        [DataMember]
        public Guid ChallengeId
        {
            get;
            private set;
        }

        protected override IValidator CreateValidator()
        {
            var validator = new ConstraintValidator<AcceptChallengeCommand>();

            validator.VerifyThat(m => m.ChallengeId).IsNotEmpty();

            return validator;
        }
    }
}
