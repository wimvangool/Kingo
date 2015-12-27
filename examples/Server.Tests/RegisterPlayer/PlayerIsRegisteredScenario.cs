using System;
using System.Threading.Tasks;
using Kingo.Constraints;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Players;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.RegisterPlayer
{
    [TestClass]
    public sealed class PlayerIsRegisteredScenario : WriteOnlyScenario<RegisterPlayerCommand>
    {
        public PlayerRegisteredEvent PlayerRegisteredEvent
        {
            get { return (PlayerRegisteredEvent) PublishedEvents[0]; }
        }

        protected override RegisterPlayerCommand When()
        {
            return new RegisterPlayerCommand(Guid.NewGuid(), "John");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Events()
                .Expect<PlayerRegisteredEvent>(Validate)
                .ExecuteAsync();
        }

        private void Validate(IMemberConstraintSet<PlayerRegisteredEvent> validator)
        {
            validator.VerifyThat(m => m.PlayerId).IsEqualTo(Message.PlayerId);
            validator.VerifyThat(m => m.PlayerName).IsEqualTo(Message.PlayerName);
            validator.VerifyThat(m => m.Version).IsEqualTo(1);
        }
    }
}
