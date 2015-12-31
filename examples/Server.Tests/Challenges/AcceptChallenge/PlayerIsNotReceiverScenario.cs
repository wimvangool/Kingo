using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Challenges.ChallengePlayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.AcceptChallenge
{
    [TestClass]
    public sealed class PlayerIsNotReceiverScenario : MemoryScenario<AcceptChallengeCommand>
    {
        public readonly PlayerIsChallengedScenario PlayerIsChallenged;

        public PlayerIsNotReceiverScenario()
        {
            PlayerIsChallenged = new PlayerIsChallengedScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return PlayerIsChallenged;
        }

        protected override AcceptChallengeCommand When()
        {
            return new AcceptChallengeCommand(PlayerIsChallenged.PlayerChallengedEvent.ChallengeId);
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
