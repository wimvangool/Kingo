using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Players.RegisterPlayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.ChallengePlayer
{
    [TestClass]
    public sealed class SenderEqualToReceiverScenario : MemoryScenario<ChallengePlayerCommand>
    {
        public readonly PlayerIsRegisteredScenario PlayerIsRegistered;

        public SenderEqualToReceiverScenario()
        {
            PlayerIsRegistered = new PlayerIsRegisteredScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return PlayerIsRegistered;
        }

        protected override ChallengePlayerCommand When()
        {
            return new ChallengePlayerCommand(Guid.NewGuid(), PlayerIsRegistered.PlayerRegisteredEvent.PlayerId);
        }

        protected override Session CreateSession()
        {
            var player = PlayerIsRegistered.PlayerRegisteredEvent;

            return new Session(player.PlayerId, player.PlayerName);
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
