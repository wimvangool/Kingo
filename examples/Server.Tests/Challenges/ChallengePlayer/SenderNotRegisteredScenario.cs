using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Players.RegisterPlayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.ChallengePlayer
{
    [TestClass]
    public sealed class SenderNotRegisteredScenario : WriteOnlyScenario<ChallengePlayerCommand>
    {
        private readonly PlayerIsRegisteredScenario _playerIsRegistered;

        public SenderNotRegisteredScenario()
        {
            _playerIsRegistered = new PlayerIsRegisteredScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return _playerIsRegistered;
        }

        protected override ChallengePlayerCommand When()
        {
            return new ChallengePlayerCommand(Guid.NewGuid(), _playerIsRegistered.PlayerRegisteredEvent.PlayerId);
        }

        protected override Session CreateSession()
        {
            return new Session(Guid.NewGuid(), "Sender");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
