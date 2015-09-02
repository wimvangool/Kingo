using System;
using System.Collections.Generic;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Constraints;
using Kingo.BuildingBlocks.Messaging.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Players
{
    [TestClass]
    public sealed class ChallengeSenderNotFoundScenario : UnitTestScenario<ChallengePlayerCommand>
    {
        private readonly PlayerIsRegisteredScenario _receiverIsRegistered;

        public ChallengeSenderNotFoundScenario()
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
            VerifyThatExceptionIsA<InvalidMessageException>().And(v1 =>
            {
                v1.VerifyThat(exception => exception.InnerException).IsInstanceOf<AggregateNotFoundByKeyException<Guid>>().And(v2 =>
                {
                    v2.VerifyThat(exception => exception.AggregateKey).IsEqualTo(Message.SenderId);
                });
            });
        }        
    }
}