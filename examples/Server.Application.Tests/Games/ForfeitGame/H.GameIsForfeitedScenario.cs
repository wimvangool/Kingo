using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Constraints;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.ForfeitGame
{
    [TestClass]
    public sealed class GameIsForfeitedScenario : InMemoryScenario<ForfeitGameCommand>
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

        protected override MessageToHandle<ForfeitGameCommand> When()
        {
            var message = new ForfeitGameCommand(GameIsStarted.GameStartedEvent.GameId);
            var playerId = GameIsStarted.WhitePlayerId;
            var playerName = GameIsStarted.WhitePlayerName;
            return new SecureMessage<ForfeitGameCommand>(message, playerId, playerName);
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
