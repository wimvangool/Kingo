using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Kings
{
    [TestClass]
    public sealed class CastlingShortOfWhiteBlockedByOwnPieceScenario : MovePieceScenario
    {
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
