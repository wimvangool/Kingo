using System;
using System.Runtime.Serialization;
using Kingo.Messaging.Validation;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class MovePieceCommand : RequestMessage
    {
        public MovePieceCommand(Guid gameId, string from, string to)
        {
            GameId = gameId;
            From = from;
            To = to;
        }

        [DataMember]
        public Guid GameId
        {
            get;
            private set;
        }

        [DataMember]
        public string From
        {
            get;
            private set;
        }

        [DataMember]
        public string To
        {
            get;
            private set;
        }

        protected override IRequestMessageValidator CreateMessageValidator() =>
            base.CreateMessageValidator().Append(CreateConstraintValidator());

        private static IRequestMessageValidator<MovePieceCommand> CreateConstraintValidator()
        {
            var validator = new ConstraintValidator<MovePieceCommand>();
            validator.VerifyThat(m => m.GameId).IsNotEmpty();
            validator.VerifyThat(m => m.From).IsNotNull().IsValidSquare();
            validator.VerifyThat(m => m.To).IsNotNull().IsValidSquare();
            validator.VerifyThat(m => m.To).IsNotEqualTo(m => m.From);
            return validator;
        }
    }
}
