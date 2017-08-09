using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Validation;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Users
{    
    [DataContract]
    public sealed class RegisterUserCommand : RequestMessage
    {                
        public RegisterUserCommand(Guid userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }

        [DataMember]
        public Guid UserId
        {
            get;
            private set;
        }

        [DataMember]
        public string UserName
        {
            get;
            private set;
        }

        protected override IRequestMessageValidator CreateMessageValidator() =>
            base.CreateMessageValidator().Append(CreateConstraintValidator());

        /// <inheritdoc />
        private static IRequestMessageValidator<RegisterUserCommand> CreateConstraintValidator()
        {
            var validator = new ConstraintValidator<RegisterUserCommand>();
            validator.VerifyThat(m => m.UserId).IsNotEmpty();
            validator.VerifyThat(m => m.UserName).IsNotNull();
            validator.VerifyThat(m => m.UserName).IsIdentifier(ErrorMessages.RegisterUserCommand_InvalidPlayerName);
            return validator;
        }
    }
}