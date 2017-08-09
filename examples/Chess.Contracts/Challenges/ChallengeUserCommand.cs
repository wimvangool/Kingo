using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Validation;

namespace Kingo.Samples.Chess.Challenges
{
    [DataContract]
    public sealed class ChallengeUserCommand : RequestMessage
    {        
        public ChallengeUserCommand(Guid challengeId, Guid senderId, Guid receiverId)
        {
            ChallengeId = challengeId;
            UserId = userId;
        }

        [DataMember]
        public Guid ChallengeId
        {
            get;
            private set;
        }

        [DataMember]
        public Guid UserId
        {
            get;
            private set;
        }

        protected override IRequestMessageValidator CreateMessageValidator() =>
            base.CreateMessageValidator().Append(CreateConstraintValidator());

        private static IRequestMessageValidator<ChallengeUserCommand> CreateConstraintValidator()
        {
            var validator = new ConstraintValidator<ChallengeUserCommand>();
            validator.VerifyThat(m => m.ChallengeId).IsNotEmpty();
            validator.VerifyThat(m => m.UserId).IsNotEmpty();
            return validator;
        }
    }
}
