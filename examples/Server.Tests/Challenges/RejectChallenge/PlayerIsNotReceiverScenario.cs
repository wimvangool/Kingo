using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Challenges.ChallengePlayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.RejectChallenge
{
    [TestClass]
    public sealed class PlayerIsNotReceiverScenario : MemoryScenario<RejectChallengeCommand>
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

        protected override RejectChallengeCommand When()
        {
            return new RejectChallengeCommand(PlayerIsChallenged.PlayerChallengedEvent.ChallengeId);
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
