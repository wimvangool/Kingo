using System;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class ForfeitGameCommand : Message
    {
        [DataMember]
        public readonly Guid GameId;

        public ForfeitGameCommand(Guid gameId)
        {
            GameId = gameId;
        }

        protected override IValidator CreateValidator()
        {
            var validator = new ConstraintValidator<ForfeitGameCommand>();

            validator.VerifyThat(m => m.GameId).IsNotEmpty();

            return validator;
        }
    }
}
