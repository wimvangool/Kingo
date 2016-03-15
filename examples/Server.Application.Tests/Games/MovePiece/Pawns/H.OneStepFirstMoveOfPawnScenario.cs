using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Pawns
{
    [TestClass]
    public sealed class OneStepFirstMoveOfPawnScenario : MovePieceScenario
    {                     
        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("e2", "e3");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
