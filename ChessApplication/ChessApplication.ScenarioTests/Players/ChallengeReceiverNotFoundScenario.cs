using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceComponents.ComponentModel;
using ServiceComponents.ComponentModel.Constraints;
using ServiceComponents.ComponentModel.Server;
using ServiceComponents.ComponentModel.Server.Domain;

namespace ServiceComponents.ChessApplication.Players
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
                .IsInstanceOf<AggregateNotFoundByKeyException<Guid>>()
                .And(ContainsExpectedAggregateKey);
        }

        private void ContainsExpectedAggregateKey(IMemberConstraintSet validator, AggregateNotFoundByKeyException<Guid> exception)
        {
            validator.VerifyThat(() => exception.AggregateKey).IsEqualTo(Message.ReceiverId);
        }
    }
}