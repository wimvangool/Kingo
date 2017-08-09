using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece
{
    [TestClass]
    public sealed class GameNotFoundScenario : UnitTest<MovePieceCommand>
    {        
        protected override MessageToHandle<MovePieceCommand> When()
        {
            return new MovePieceCommand(Guid.NewGuid(), "e2", "e4");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
