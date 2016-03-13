using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Rooks
{
    [TestClass]
    public sealed class TwoStepsFowardScenario : MovePieceScenario
    {
        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return base.Given().Concatenate();
            yield return WhitePlayerMove("a2", "a4");
            yield return BlackPlayerMove("h7", "h5");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("a1", "a3");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
