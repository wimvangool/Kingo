using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Check
{

    [TestClass]
    public sealed class SelfCheckByKnightScenario : MovePieceScenario
    {
        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return base.Given().Concatenate();
            yield return WhitePlayerMove("g1", "f3");
            yield return BlackPlayerMove("d7", "d6");
            yield return WhitePlayerMove("f3", "h4");
            yield return BlackPlayerMove("e7", "e5");
            yield return WhitePlayerMove("h4", "f5");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return BlackPlayerMove("e8", "e7");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
