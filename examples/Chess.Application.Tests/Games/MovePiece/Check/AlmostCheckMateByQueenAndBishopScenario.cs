using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Check
{
    [TestClass]
    public sealed class AlmostCheckMateByQueenAndBishopScenario : MovePieceScenario
    {
        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return base.Given().Concatenate();
            yield return WhitePlayerMove("e2", "e4");
            yield return BlackPlayerMove("e7", "e5");
            yield return WhitePlayerMove("f1", "c4");
            yield return BlackPlayerMove("b8", "a6");
            yield return WhitePlayerMove("d1", "f3");
            yield return BlackPlayerMove("g8", "h6");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("f3", "f7");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent(GameState.Check);
        }
    }
}
