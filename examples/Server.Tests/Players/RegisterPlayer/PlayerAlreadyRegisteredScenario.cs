using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Players.RegisterPlayer
{
    [TestClass]
    public sealed class PlayerAlreadyRegisteredScenario : WriteOnlyScenario<RegisterPlayerCommand>
    {
        private readonly PlayerIsRegisteredScenario _playerIsRegistered;

        public PlayerAlreadyRegisteredScenario()
        {
            _playerIsRegistered = new PlayerIsRegisteredScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return _playerIsRegistered;
        }

        protected override RegisterPlayerCommand When()
        {
            return new RegisterPlayerCommand(Guid.NewGuid(), _playerIsRegistered.Message.PlayerName);
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
