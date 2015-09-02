using System.Collections.Generic;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Constraints;
using Kingo.ChessApplication.Challenges;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Players
{
    [TestClass]
    public sealed class PlayerIsChallengedScenario : UnitTestScenario<ChallengePlayerCommand>
    {
        public readonly PlayerIsRegisteredScenario SenderIsRegistered;
        public readonly PlayerIsRegisteredScenario ReceiverIsRegistered;

        public PlayerIsChallengedScenario()
        {
            SenderIsRegistered = new PlayerIsRegisteredScenario();
            ReceiverIsRegistered = new PlayerIsRegisteredScenario();
        }

        public PlayerChallengedEvent PlayerChallengedEvent
        {
            get { return GetDomainEventAt<PlayerChallengedEvent>(0); }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return SenderIsRegistered;
            yield return ReceiverIsRegistered;
        }

        protected override ChallengePlayerCommand When()
        {
            var senderId = SenderIsRegistered.PlayerRegisteredEvent.PlayerId;
            var receiverId = ReceiverIsRegistered.PlayerRegisteredEvent.PlayerId;

            return new ChallengePlayerCommand(senderId, receiverId);
        }

        [TestMethod]
        public override void Then()
        {
            VerifyThatDomainEventCount().IsEqualTo(1);
            VerifyThatDomainEventAtIndex(0).IsInstanceOf<PlayerChallengedEvent>().And(validator =>
            {
                validator.VerifyThat(e => e.ChallengeId).IsNotEmpty();
                validator.VerifyThat(e => e.SenderId).IsEqualTo(Message.SenderId);
                validator.VerifyThat(e => e.ReceiverId).IsEqualTo(Message.ReceiverId);
            });
        }
    }
}