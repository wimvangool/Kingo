using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Queens
{
    [TestClass]
    public sealed class FourStepsUpLeftScenario : MovePieceScenario
    {
        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return base.Given().Concatenate();
            yield return WhitePlayerMove("c2", "c3");
            yield return BlackPlayerMove("c7", "c6");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("d1", "a4");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
