using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Constraints;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Pawns
{
    [TestClass]
    public sealed class PawnMovedToEightRankScenario : MovePieceScenario
    {        
        protected override IEnumerable<IMessageSequence> Given()
        {
            #region [====== Creating Free Path to Eight Rank for White Pawn ======]

            yield return base.Given().Concatenate();

            yield return WhitePlayerMove("a2", "a4");
            yield return BlackPlayerMove("b7", "b5");

            yield return WhitePlayerMove("a4", "b5");
            yield return BlackPlayerMove("b8", "c6");

            yield return WhitePlayerMove("a1", "a7");
            yield return BlackPlayerMove("a8", "a7");

            #endregion

            #region [====== Walking towards Eight Rank ======]

            yield return WhitePlayerMove("b5", "b6");
            yield return BlackPlayerMove("c8", "a6");

            yield return WhitePlayerMove("b6", "b7");
            yield return BlackPlayerMove("d7", "d5");

            #endregion
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("b7", "b8");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedEvent<PawnMovedToEightRankEvent>(Validate);
        }

        private void Validate(IMemberConstraintSet<PawnMovedToEightRankEvent> validator)
        {
            validator.VerifyThat(m => m.GameId).IsEqualTo(GameId);
            validator.VerifyThat(m => m.GameVersion).IsGreaterThan(0);
            validator.VerifyThat(m => m.From).IsEqualTo(Message.From);
            validator.VerifyThat(m => m.To).IsEqualTo(Message.To);
            validator.VerifyThat(m => m.NewState).IsEqualTo(GameState.AwaitingPawnPromotion);
        }
    }
}
