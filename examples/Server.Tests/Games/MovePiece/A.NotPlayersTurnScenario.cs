using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece
{
    [TestClass]
    public sealed class NotPlayersTurnScenario : MovePieceScenario
    {        
        protected override MessageToHandle<MovePieceCommand> When()
        {
            return BlackPlayerMove("e7", "e5");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
