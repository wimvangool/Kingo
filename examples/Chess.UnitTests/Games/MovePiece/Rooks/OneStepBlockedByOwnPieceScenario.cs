using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Rooks
{
    [TestClass]
    public sealed class OneStepBlockedByOwnPieceScenario : MovePieceScenario
    {
        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("h1", "h2");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
