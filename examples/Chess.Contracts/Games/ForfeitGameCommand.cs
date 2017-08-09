using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Validation;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class ForfeitGameCommand : RequestMessage
    {       
        public ForfeitGameCommand(Guid gameId)
        {
            GameId = gameId;
        }

        [DataMember]
        public Guid GameId
        {
            get;
            private set;
        }

        protected override IRequestMessageValidator CreateMessageValidator() =>
            base.CreateMessageValidator().Append(CreateConstraintValidator());

        private static IRequestMessageValidator<ForfeitGameCommand> CreateConstraintValidator()
        {
            var validator = new ConstraintValidator<ForfeitGameCommand>();
            validator.VerifyThat(m => m.GameId).IsNotEmpty();
            return validator;
        }
    }
}
