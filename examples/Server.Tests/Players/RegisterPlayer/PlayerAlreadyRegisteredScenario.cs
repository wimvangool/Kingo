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
        public readonly PlayerIsRegisteredScenario PlayerIsRegistered;

        public PlayerAlreadyRegisteredScenario()
        {
            PlayerIsRegistered = new PlayerIsRegisteredScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return PlayerIsRegistered;
        }

        protected override RegisterPlayerCommand When()
        {
            return new RegisterPlayerCommand(Guid.NewGuid(), PlayerIsRegistered.Message.PlayerName);
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
