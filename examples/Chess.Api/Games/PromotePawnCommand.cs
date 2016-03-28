using System;
using System.Runtime.Serialization;
using Kingo.Constraints;
using Kingo.Messaging;

namespace Kingo.Samples.Chess.Games
{
    [DataContract]
    public sealed class PromotePawnCommand : Message
    {
        public PromotePawnCommand(Guid gameId, TypeOfPiece promoteTo)
        {
            GameId = gameId;
            PromoteTo = promoteTo;
        }

        [DataMember]
        public Guid GameId
        {
            get;
            private set;
        }

        [DataMember]
        public TypeOfPiece PromoteTo
        {
            get;
            private set;
        }

        protected override IValidator CreateValidator()
        {
            var validator = new ConstraintValidator<PromotePawnCommand>();

            validator.VerifyThat(m => m.GameId).IsNotEmpty();
            validator.VerifyThat(m => m.PromoteTo)
                .IsInRangeOfValidValues()
                .IsNotEqualTo(TypeOfPiece.Pawn)
                .IsNotEqualTo(TypeOfPiece.King);

            return validator;
        }
    }
}
