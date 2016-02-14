using System;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Kingo.Messaging;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Players
{    
    [DataContract]
    public sealed class RegisterPlayerCommand : Message
    {                
        public RegisterPlayerCommand(Guid playerId, string playerName)
        {
            PlayerId = playerId;
            PlayerName = playerName;
        }

        [DataMember]
        public Guid PlayerId
        {
            get;
            private set;
        }

        [DataMember]
        public string PlayerName
        {
            get;
            private set;
        }

        /// <inheritdoc />
        protected override IValidator CreateValidator()
        {
            var validator = new ConstraintValidator<RegisterPlayerCommand>();

            validator.VerifyThat(m => m.PlayerId).IsNotEmpty(ErrorMessages.Message_ValueNotSpecified);
            validator.VerifyThat(m => m.PlayerName).IsNotNull(ErrorMessages.Message_ValueNotSpecified);
            validator.VerifyThat(m => m.PlayerName).IsIdentifier(ErrorMessages.RegisterPlayerCommand_InvalidPlayerName);

            return validator;
        }
    }
}