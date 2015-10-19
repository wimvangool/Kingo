using System;
using System.Collections.Generic;
using Kingo.BuildingBlocks.Constraints;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Players
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
            VerifyThatExceptionIsA<InvalidMessageException>().And(v1 =>
            {
                v1.VerifyThat(exception => exception.InnerException).IsInstanceOf<AggregateNotFoundByKeyException<Guid>>().And(v2 =>
                {
                    v2.VerifyThat(exception => exception.AggregateKey).IsEqualTo(Message.ReceiverId);
                });
            });
        }        
    }
}