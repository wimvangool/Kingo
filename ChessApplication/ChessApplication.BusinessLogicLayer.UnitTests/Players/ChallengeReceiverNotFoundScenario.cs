using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syztem.ComponentModel;
using Syztem.ComponentModel.FluentValidation;
using Syztem.ComponentModel.Server;
using Syztem.ComponentModel.Server.Domain;

namespace SummerBreeze.ChessApplication.Players
{
    [TestClass]
    public sealed class ChallengeReceiverNotFoundScenario : UnitTestScenario<ChallengePlayerCommand>
    {	    
        private readonly PlayerIsRegisteredScenario _senderIsRegistered;

        public ChallengeReceiverNotFoundScenario()
        {
            _senderIsRegistered = new PlayerIsRegisteredScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return _senderIsRegistered;
        }

        protected override ChallengePlayerCommand When()
        {
            var senderId = _senderIsRegistered.PlayerRegisteredEvent.PlayerId;
            var receiverId = Guid.NewGuid();

            return new ChallengePlayerCommand(senderId, receiverId);
        }

        [TestMethod]
        public override void Then()
        {
            VerifyThatExceptionIsA<InvalidMessageException>().And(ContainsExpectedInnerException);
        }

        private void ContainsExpectedInnerException(IMemberConstraintSet validator, InvalidMessageException exception)
        {
            validator.VerifyThat(() => exception.InnerException)
                .IsInstanceOf<AggregateNotFoundByKeyException<Player, Guid>>()
                .And(ContainsExpectedAggregateKey);
        }

        private void ContainsExpectedAggregateKey(IMemberConstraintSet validator, AggregateNotFoundByKeyException<Player, Guid> exception)
        {
            validator.VerifyThat(() => exception.AggregateKey).IsEqualTo(Message.ReceiverId);
        }
    }
}