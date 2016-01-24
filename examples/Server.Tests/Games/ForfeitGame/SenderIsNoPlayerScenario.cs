using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.ForfeitGame
{
    [TestClass]
    public sealed class SenderIsNoPlayerScenario : InMemoryScenario<ForfeitGameCommand>
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

        protected override ForfeitGameCommand When()
        {
            return new ForfeitGameCommand(GameIsStarted.GameStartedEvent.GameId);
        }

        protected override Session CreateSession()
        {
            return new Session(Guid.NewGuid(), "SomePlayer");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
