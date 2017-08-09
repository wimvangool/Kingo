using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Validation;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class AcceptChallengeCommand : RequestMessage
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

        protected override IRequestMessageValidator CreateMessageValidator() =>
            base.CreateMessageValidator().Append(CreateConstraintValidator());

        private static IRequestMessageValidator<AcceptChallengeCommand> CreateConstraintValidator()
        {
            var validator = new ConstraintValidator<AcceptChallengeCommand>();
            validator.VerifyThat(m => m.ChallengeId).IsNotEmpty();
            return validator;
        }
    }
}
