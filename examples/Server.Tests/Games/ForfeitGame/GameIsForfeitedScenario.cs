using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Constraints;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.ForfeitGame
{
    [TestClass]
    public sealed class GameIsForfeitedScenario : MemoryScenario<ForfeitGameCommand>
    {
        public readonly GameIsStartedScenario GameIsStarted;

        public GameIsForfeitedScenario()
        {
            GameIsStarted = new GameIsStartedScenario();
        }

        public GameForfeitedEvent GameForfeitedEvent
        {
            get { return (GameForfeitedEvent) PublishedEvents[0]; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return GameIsStarted;
        }

        protected override ForfeitGameCommand When()
        {
            return new ForfeitGameCommand(GameIsStarted.GameStartedEvent.GameId);
        }

        protected override Session CreateSession()
        {
            var playerId = GameIsStarted.WhitePlayerId;
            var playerName = GameIsStarted.WhitePlayerName;

            return new Session(playerId, playerName);
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Events().Expect<GameForfeitedEvent>(Validate).ExecuteAsync();
        }

        private void Validate(IMemberConstraintSet<GameForfeitedEvent> validator)
        {
            validator.VerifyThat(m => m.GameId).IsEqualTo(Message.GameId);
            validator.VerifyThat(m => m.GameVersion).IsGreaterThan(1);
        }
    }
}
