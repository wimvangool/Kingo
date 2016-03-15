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
    public sealed class PlayerIsChallengedScenario : InMemoryScenario<ChallengePlayerCommand>
    {
        public readonly PlayerIsRegisteredScenario SenderIsRegistered;
        public readonly PlayerIsRegisteredScenario ReceiverIsRegistered;

        public PlayerIsChallengedScenario()
        {
            SenderIsRegistered = new PlayerIsRegisteredScenario("Wim");
            ReceiverIsRegistered = new PlayerIsRegisteredScenario("Peter");
        }

        public PlayerChallengedEvent PlayerChallengedEvent
        {
            get { return (PlayerChallengedEvent) PublishedEvents[0]; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return SenderIsRegistered;
            yield return ReceiverIsRegistered;
        }

        protected override MessageToHandle<ChallengePlayerCommand> When()
        {
            var message = new ChallengePlayerCommand(Guid.NewGuid(), ReceiverIsRegistered.PlayerRegisteredEvent.PlayerId);
            var sender = SenderIsRegistered.PlayerRegisteredEvent;
            return new SecureMessage<ChallengePlayerCommand>(message, sender.PlayerId, sender.PlayerName);
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
            validator.VerifyThat(m => m.SenderId).IsEqualTo(SenderIsRegistered.PlayerRegisteredEvent.PlayerId);
            validator.VerifyThat(m => m.ReceiverId).IsEqualTo(ReceiverIsRegistered.PlayerRegisteredEvent.PlayerId);
        }
    }
}
