using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Validation;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class RejectChallengeCommand : RequestMessage
    {        
        public RejectChallengeCommand(Guid challengeId)
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

        private static IRequestMessageValidator<RejectChallengeCommand> CreateConstraintValidator()
        {
            var validator = new ConstraintValidator<RejectChallengeCommand>();
            validator.VerifyThat(m => m.ChallengeId).IsNotEmpty();
            return validator;
        }
    }
}
