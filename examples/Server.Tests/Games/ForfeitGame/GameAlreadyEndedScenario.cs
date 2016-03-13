using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.ForfeitGame
{
    [TestClass]
    public sealed class GameAlreadyEndedScenario : InMemoryScenario<ForfeitGameCommand>
    {
        public readonly GameIsForfeitedScenario GameIsForfeited;

        public GameAlreadyEndedScenario()
        {
            GameIsForfeited = new GameIsForfeitedScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return GameIsForfeited;
        }

        protected override MessageToHandle<ForfeitGameCommand> When()
        {
            var message = new ForfeitGameCommand(GameIsForfeited.GameForfeitedEvent.GameId);
            var playerId = GameIsForfeited.GameIsStarted.WhitePlayerId;
            var playerName = GameIsForfeited.GameIsStarted.WhitePlayerName;
            return new SecureMessage<ForfeitGameCommand>(message, playerId, playerName);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
