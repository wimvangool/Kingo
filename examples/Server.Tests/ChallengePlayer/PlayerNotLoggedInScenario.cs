using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Challenges;
using Kingo.Samples.Chess.RegisterPlayer;
using Kingo.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.ChallengePlayer
{
    [TestClass]
    public sealed class PlayerNotLoggedInScenario : WriteOnlyScenario<ChallengePlayerCommand>
    {
        private readonly PlayerIsRegisteredScenario _playerIsRegistered;

        public PlayerNotLoggedInScenario()
        {
            _playerIsRegistered = new PlayerIsRegisteredScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return _playerIsRegistered;
        }

        protected override ChallengePlayerCommand When()
        {
            return new ChallengePlayerCommand(Guid.NewGuid(), _playerIsRegistered.PlayerRegisteredEvent.PlayerName);
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<AuthorizationException>().ExecuteAsync();
        }
    }
}
