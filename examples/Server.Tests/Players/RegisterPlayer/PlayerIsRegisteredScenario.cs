using System;
using System.Threading.Tasks;
using Kingo.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Players.RegisterPlayer
{
    [TestClass]
    public sealed class PlayerIsRegisteredScenario : InMemoryScenario<RegisterPlayerCommand>
    {
        private readonly string _playerName;

        public PlayerIsRegisteredScenario()
            : this("John") { }

        public PlayerIsRegisteredScenario(string playerName)
        {
            if (playerName == null)
            {
                throw new ArgumentNullException("playerName");
            }
            _playerName = playerName;
        }

        public PlayerRegisteredEvent PlayerRegisteredEvent
        {
            get { return (PlayerRegisteredEvent) PublishedEvents[0]; }
        }

        protected override RegisterPlayerCommand When()
        {
            return new RegisterPlayerCommand(Guid.NewGuid(), _playerName);
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
            validator.VerifyThat(m => m.PlayerVersion).IsEqualTo(1);
            validator.VerifyThat(m => m.PlayerName).IsEqualTo(Message.PlayerName);            
        }
    }
}
