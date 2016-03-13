using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Pawns
{
    [TestClass]
    public sealed class TwoStepFirstMoveOfPawnScenario : MovePieceScenario
    {                     
        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("e2", "e4");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
