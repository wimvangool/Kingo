using System;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Constraints;
using Kingo.BuildingBlocks.Messaging.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Challenges
{
    [TestClass]
    public sealed class RejectedChallengeNotFoundScenario : UnitTestScenario<RejectChallengeCommand>
    {	    
        protected override RejectChallengeCommand When()
        {
            return new RejectChallengeCommand(Guid.NewGuid());
        }

        [TestMethod]
        public override void Then()
        {
            VerifyThatExceptionIsA<InvalidMessageException>().And(v1 =>
            {
                v1.VerifyThat(exception => exception.InnerException).IsInstanceOf<AggregateNotFoundByKeyException<Guid>>().And(v2 =>
                {
                    v2.VerifyThat(exception => exception.AggregateKey).IsEqualTo(Message.ChallengeId);
                });
            });
        }       
    }
}