using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece
{
    [TestClass]
    public sealed class EmptySquareScenario : MovePieceScenario
    {        
        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("e3", "e4");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
