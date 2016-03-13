using System;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Kingo.Messaging;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class MovePieceCommand : Message
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

        protected override IValidator CreateValidator()
        {
            var validator = new ConstraintValidator<MovePieceCommand>();

            validator.VerifyThat(m => m.GameId).IsNotEmpty();
            validator.VerifyThat(m => m.From).IsNotNull(ErrorMessages.Message_ValueNotSpecified).IsValidSquare();
            validator.VerifyThat(m => m.To).IsNotNull(ErrorMessages.Message_ValueNotSpecified).IsValidSquare();

            return validator;
        }
    }
}
