using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Bishops
{
    [TestClass]
    public sealed class OneStepBlockedByOwnPawnScenario : MovePieceScenario
    {
        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("c1", "b2");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
