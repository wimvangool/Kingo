using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceComponents.ComponentModel;
using ServiceComponents.ComponentModel.Constraints;
using ServiceComponents.ComponentModel.Server;

namespace ServiceComponents.ChessApplication.Challenges
{
    [TestClass]
    public sealed class RejectedChallengeAlreadyRejectedScenario : UnitTestScenario<RejectChallengeCommand>
    {
        private readonly ChallengeIsRejectedScenario _challengeIsRejected;

        public RejectedChallengeAlreadyRejectedScenario()
        {
            _challengeIsRejected = new ChallengeIsRejectedScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return _challengeIsRejected;
        }

        protected override RejectChallengeCommand When()
        {
            return new RejectChallengeCommand(_challengeIsRejected.ChallengeRejectedEvent.ChallengeId);
        }

        [TestMethod]
        public override void Then()
        {
            VerifyThatExceptionIsA<CommandExecutionException>().And(ContainsExpectedInnerException);
        }

        private void ContainsExpectedInnerException(IMemberConstraintSet validator, CommandExecutionException exception)
        {
            validator.VerifyThat(() => exception.InnerException)
                .IsInstanceOf<ChallengeAlreadyRejectedException>()
                .And(ContainsExpectedChallengeId);
        }

        private void ContainsExpectedChallengeId(IMemberConstraintSet validator, ChallengeAlreadyRejectedException exception)
        {
            validator.VerifyThat(() => exception.ChallengeId).IsEqualTo(Message.ChallengeId);
        }
    }
}