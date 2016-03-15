using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece
{
    [TestClass]
    public sealed class SenderIsNoPlayerScenario : MovePieceScenario
    {                
        protected override MessageToHandle<MovePieceCommand> When()
        {
            var message = new MovePieceCommand(GameIsStarted.GameStartedEvent.GameId, "e2", "e4");
            var session = RandomSession();
            return new SecureMessage<MovePieceCommand>(message, session);
        }                

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
