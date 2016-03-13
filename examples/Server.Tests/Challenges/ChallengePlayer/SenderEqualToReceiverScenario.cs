using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Players.RegisterPlayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.ChallengePlayer
{
    [TestClass]
    public sealed class SenderEqualToReceiverScenario : InMemoryScenario<ChallengePlayerCommand>
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

        protected override MessageToHandle<ChallengePlayerCommand> When()
        {
            var message = new ChallengePlayerCommand(Guid.NewGuid(), PlayerIsRegistered.PlayerRegisteredEvent.PlayerId);
            var player = PlayerIsRegistered.PlayerRegisteredEvent;            
            return new SecureMessage<ChallengePlayerCommand>(message, player.PlayerId, player.PlayerName);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
