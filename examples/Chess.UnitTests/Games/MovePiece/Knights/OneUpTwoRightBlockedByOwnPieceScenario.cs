using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Knights
{
    [TestClass]
    public sealed class OneUpTwoRightBlockedByOwnPieceScenario : MovePieceScenario
    {
        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("b1", "d2");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
