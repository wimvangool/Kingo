using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.ForfeitGame
{
    [TestClass]
    public sealed class SenderIsNoPlayerScenario : UnitTest<ForfeitGameCommand>
    {
        public readonly GameIsStartedScenario GameIsStarted;

        public SenderIsNoPlayerScenario()
        {
            GameIsStarted = new GameIsStartedScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return GameIsStarted;
        }

        protected override MessageToHandle<ForfeitGameCommand> When()
        {
            var message = new ForfeitGameCommand(GameIsStarted.GameStartedEvent.GameId);
            var session = RandomSession();
            return new SecureMessage<ForfeitGameCommand>(message, session);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
