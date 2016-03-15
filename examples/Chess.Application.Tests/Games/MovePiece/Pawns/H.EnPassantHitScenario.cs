using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Constraints;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Pawns
{
    [TestClass]
    public sealed class EnPassantHitScenario : MovePieceScenario
    {
        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return base.Given().Concatenate();
            yield return WhitePlayerMove("e2", "e4");
            yield return BlackPlayerMove("a7", "a6");
            yield return WhitePlayerMove("e4", "e5");
            yield return BlackPlayerMove("f7", "f5");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("e5", "f6");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedEvent<EnPassantHitEvent>(Validate);
        }

        private void Validate(IMemberConstraintSet<EnPassantHitEvent> validator)
        {
            validator.VerifyThat(m => m.GameId).IsEqualTo(GameId);
            validator.VerifyThat(m => m.GameVersion).IsGreaterThan(0);
            validator.VerifyThat(m => m.From).IsEqualTo(Message.From);
            validator.VerifyThat(m => m.To).IsEqualTo(Message.To);
            validator.VerifyThat(m => m.EnPassantHit).IsEqualTo("f5");
            validator.VerifyThat(m => m.NewState).IsEqualTo(GameState.Normal);
        }
    }
}
