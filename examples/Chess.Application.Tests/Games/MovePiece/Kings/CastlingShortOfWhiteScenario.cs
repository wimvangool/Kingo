using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Constraints;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Kings
{
    [TestClass]
    public sealed class CastlingShortOfWhiteScenario : MovePieceScenario
    {
        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return base.Given().Concatenate();
            yield return WhitePlayerMove("e2", "e4");
            yield return BlackPlayerMove("e7", "e5");
            yield return WhitePlayerMove("d2", "d3");
            yield return BlackPlayerMove("g8", "f6");
            yield return WhitePlayerMove("g1", "f3");
            yield return BlackPlayerMove("g7", "g6");
            yield return WhitePlayerMove("g2", "g3");
            yield return BlackPlayerMove("b8", "c6");
            yield return WhitePlayerMove("f1", "g2");
            yield return BlackPlayerMove("f8", "c5");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("e1", "g1");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedEvent<CastlingPerformedEvent>(Validate);
        }

        private void Validate(IMemberConstraintSet<CastlingPerformedEvent> validator)
        {
            validator.VerifyThat(m => m.GameId).IsEqualTo(GameId);
            validator.VerifyThat(m => m.GameVersion).IsGreaterThan(0);
            validator.VerifyThat(m => m.From).IsEqualTo(Message.From);
            validator.VerifyThat(m => m.To).IsEqualTo(Message.To);
            validator.VerifyThat(m => m.NewState).IsEqualTo(GameState.Normal);
            validator.VerifyThat(m => m.RookFrom).IsEqualTo("h1");
            validator.VerifyThat(m => m.RookTo).IsEqualTo("f1");
        }
    }
}
