using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Kings
{
    [TestClass]
    public sealed class OneStepUpScenario : MovePieceScenario
    {
        protected override IEnumerable<IMessageSequence> Given()
        {
            // All Pawns in front of the king are moved towards the midfield.
            yield return base.Given().Concatenate();
            yield return WhitePlayerMove("d2", "d4");
            yield return BlackPlayerMove("d7", "d5");
            yield return WhitePlayerMove("e2", "e4");
            yield return BlackPlayerMove("e7", "e5");
            yield return WhitePlayerMove("f2", "f4");
            yield return BlackPlayerMove("f7", "f5");

            // Adjacent Queen and Bishop are also moved out of the way, so that
            // the King can move freely in any direction.
            yield return WhitePlayerMove("d1", "g4");
            yield return BlackPlayerMove("d8", "g5");

            yield return WhitePlayerMove("f1", "c4");
            yield return BlackPlayerMove("f8", "c5");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("e1", "e2");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
