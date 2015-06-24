using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.FluentValidation;
using System.ComponentModel.Server;
using System.ComponentModel.Server.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SummerBreeze.ChessApplication.Players
{
    [TestClass]
    public sealed class SenderNotFoundScenario : UnitTestScenario<ChallengePlayerCommand>
    {
        private readonly PlayerIsRegisteredScenario _receiverIsRegistered;

        public SenderNotFoundScenario()
        {
            _receiverIsRegistered = new PlayerIsRegisteredScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return _receiverIsRegistered;
        }

        protected override ChallengePlayerCommand When()
        {
            var senderId = Guid.NewGuid();
            var receiverId = _receiverIsRegistered.PlayerRegisteredEvent.PlayerId;

            return new ChallengePlayerCommand(senderId, receiverId);
        }

        [TestMethod]
        public override void Then()
        {
            VerifyThatExceptionIsA<InvalidMessageException>().And(ContainsExpectedInnerException);
        }

        private void ContainsExpectedInnerException(IFluentValidator validator, InvalidMessageException exception)
        {
            validator.VerifyThat(() => exception.InnerException)
                .IsInstanceOf<AggregateNotFoundByKeyException<Player, Guid>>()
                .And(ContainsExpectedAggregateKey);
        }

        private void ContainsExpectedAggregateKey(IFluentValidator validator, AggregateNotFoundByKeyException<Player, Guid> exception)
        {
            validator.VerifyThat(() => exception.AggregateKey).IsEqualTo(Message.SenderId);
        }
    }
}