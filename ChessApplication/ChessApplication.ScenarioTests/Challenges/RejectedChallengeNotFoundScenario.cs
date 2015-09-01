using System;
using Kingo.BuildingBlocks.ComponentModel;
using Kingo.BuildingBlocks.ComponentModel.Constraints;
using Kingo.BuildingBlocks.ComponentModel.Server.Domain;
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
            VerifyThatExceptionIsA<InvalidMessageException>().And(ContainsExpectedInnerException);
        }

        private void ContainsExpectedInnerException(IMemberConstraintSet validator, InvalidMessageException exception)
        {
            validator.VerifyThat(() => exception.InnerException)
                .IsInstanceOf<AggregateNotFoundByKeyException<Guid>>()
                .And(ContainsExpectedChallengeId);
        }

        private void ContainsExpectedChallengeId(IMemberConstraintSet validator, AggregateNotFoundByKeyException<Guid> exception)
        {
            validator.VerifyThat(() => exception.AggregateKey).IsEqualTo(Message.ChallengeId);
        }
    }
}