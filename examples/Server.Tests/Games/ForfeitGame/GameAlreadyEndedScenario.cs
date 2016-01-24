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

        protected override ForfeitGameCommand When()
        {
            return new ForfeitGameCommand(GameIsForfeited.GameForfeitedEvent.GameId);
        }

        protected override Session CreateSession()
        {
            var playerId = GameIsForfeited.GameIsStarted.WhitePlayerId;
            var playerName = GameIsForfeited.GameIsStarted.WhitePlayerName;

            return new Session(playerId, playerName);
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
