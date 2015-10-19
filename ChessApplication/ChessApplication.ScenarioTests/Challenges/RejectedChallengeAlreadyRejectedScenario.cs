using System.Collections.Generic;
using Kingo.BuildingBlocks.Constraints;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Challenges
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

        private void ContainsExpectedInnerException(IMemberConstraintSet<CommandExecutionException> validator)
        {
            validator.VerifyThat(exception => exception.InnerException)
                .IsInstanceOf<ChallengeAlreadyRejectedException>()
                .And(ContainsExpectedChallengeId);
        }

        private void ContainsExpectedChallengeId(IMemberConstraintSet<ChallengeAlreadyRejectedException> validator)
        {
            validator.VerifyThat(exception => exception.ChallengeId).IsEqualTo(Message.ChallengeId);
        }
    }
}