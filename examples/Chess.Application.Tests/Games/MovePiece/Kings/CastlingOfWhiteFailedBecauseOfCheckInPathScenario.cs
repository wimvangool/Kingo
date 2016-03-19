using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Kings
{
    [TestClass]
    public sealed class CastlingOfWhiteFailedBecauseOfCheckInPathScenario : MovePieceScenario
    {
        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return base.Given().Concatenate();
            yield return WhitePlayerMove("e2", "e4");
            yield return BlackPlayerMove("e7", "e5");
            yield return WhitePlayerMove("g1", "f3");
            yield return BlackPlayerMove("b7", "b6");
            yield return WhitePlayerMove("f1", "a6");
            yield return BlackPlayerMove("c8", "a6");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("e1", "g1");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
