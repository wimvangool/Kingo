using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Constraints;
using Kingo.Messaging;
using Kingo.Samples.Chess.Players.RegisterPlayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.ChallengePlayer
{
    [TestClass]
    public sealed class PlayerIsChallengedScenario : WriteOnlyScenario<ChallengePlayerCommand>
    {
        private readonly PlayerIsRegisteredScenario _senderIsRegistered;
        private readonly PlayerIsRegisteredScenario _receiverIsRegistered;

        public PlayerIsChallengedScenario()
        {
            _senderIsRegistered = new PlayerIsRegisteredScenario("Wim");
            _receiverIsRegistered = new PlayerIsRegisteredScenario("Peter");
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return _senderIsRegistered;
            yield return _receiverIsRegistered;
        }

        protected override ChallengePlayerCommand When()
        {
            return new ChallengePlayerCommand(Guid.NewGuid(), _receiverIsRegistered.PlayerRegisteredEvent.PlayerId);
        }

        protected override Session CreateSession()
        {
            var sender = _senderIsRegistered.PlayerRegisteredEvent;

            return new Session(sender.PlayerId, sender.PlayerName);
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Events().Expect<PlayerChallengedEvent>(Validate).ExecuteAsync();
        }

        private void Validate(IMemberConstraintSet<PlayerChallengedEvent> validator)
        {
            validator.VerifyThat(m => m.ChallengeId).IsEqualTo(Message.ChallengeId);
            validator.VerifyThat(m => m.ChallengeVersion).IsEqualTo(1);
            validator.VerifyThat(m => m.SenderId).IsEqualTo(_senderIsRegistered.PlayerRegisteredEvent.PlayerId);
            validator.VerifyThat(m => m.ReceiverId).IsEqualTo(_receiverIsRegistered.PlayerRegisteredEvent.PlayerId);
        }
    }
}
