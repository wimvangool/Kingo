using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.FluentValidation;
using System.ComponentModel.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SummerBreeze.ChessApplication.Players
{
    [TestClass]
    public sealed class PlayerIsChallengedScenario : UnitTestScenario<ChallengePlayerCommand>
    {
        private readonly PlayerIsRegisteredScenario _senderIsRegistered;
        private readonly PlayerIsRegisteredScenario _receiverIsRegistered;

        public PlayerIsChallengedScenario()
        {
            _senderIsRegistered = new PlayerIsRegisteredScenario();
            _receiverIsRegistered = new PlayerIsRegisteredScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return _senderIsRegistered;
            yield return _receiverIsRegistered;
        }

        protected override ChallengePlayerCommand When()
        {
            var senderId = _senderIsRegistered.PlayerRegisteredEvent.PlayerId;
            var receiverId = _receiverIsRegistered.PlayerRegisteredEvent.PlayerId;

            return new ChallengePlayerCommand(senderId, receiverId);
        }

        [TestMethod]
        public override void Then()
        {
            VerifyThatDomainEventCount().IsEqualTo(1);
            VerifyThatDomainEventAtIndex(0).IsInstanceOf<PlayerChallengedEvent>().And((validator, @event) =>
            {
                validator.VerifyThat(() => @event.ChallengeId).IsNotEmpty();
                validator.VerifyThat(() => @event.SenderId).IsEqualTo(Message.SenderId);
                validator.VerifyThat(() => @event.ReceiverId).IsEqualTo(Message.ReceiverId);
            });
        }
    }
}