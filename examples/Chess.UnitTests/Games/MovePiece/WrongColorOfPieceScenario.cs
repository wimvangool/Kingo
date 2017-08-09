using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece
{
    [TestClass]
    public sealed class WrongColorOfPieceScenario : MovePieceScenario
    {        
        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("e7", "e6");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
