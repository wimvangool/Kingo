using System;
using System.ComponentModel;
using System.ComponentModel.FluentValidation;
using System.ComponentModel.Server.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SummerBreeze.ChessApplication.Challenges
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
                .IsInstanceOf<AggregateNotFoundByKeyException<Challenge, Guid>>()
                .And(ContainsExpectedChallengeId);
        }

        private void ContainsExpectedChallengeId(IMemberConstraintSet validator, AggregateNotFoundByKeyException<Challenge, Guid> exception)
        {
            validator.VerifyThat(() => exception.AggregateKey).IsEqualTo(Message.ChallengeId);
        }
    }
}