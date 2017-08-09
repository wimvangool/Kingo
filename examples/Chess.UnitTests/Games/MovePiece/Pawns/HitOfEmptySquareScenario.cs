using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Pawns
{
    [TestClass]
    public sealed class HitOfEmptySquareScenario : MovePieceScenario
    {
        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("a2", "b3");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
