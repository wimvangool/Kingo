using System.Collections.Generic;
using Kingo.BuildingBlocks.ComponentModel;
using Kingo.BuildingBlocks.ComponentModel.Server;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Challenges
{
    [TestClass]
    public sealed class RejectedChallengeAlreadyAcceptedScenario : UnitTestScenario<RejectChallengeCommand>
    {
        private readonly ChallengeIsAcceptedScenario _challengeIsAccepted;

        public RejectedChallengeAlreadyAcceptedScenario()
        {
            _challengeIsAccepted = new ChallengeIsAcceptedScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return _challengeIsAccepted;
        }

        protected override RejectChallengeCommand When()
        {
            return new RejectChallengeCommand(_challengeIsAccepted.ChallengeAcceptedEvent.ChallengeId);
        }

        [TestMethod]
        public override void Then()
        {
            VerifyThatExceptionIsA<CommandExecutionException>().And(ContainsExpectedInnerException);
        }

        private void ContainsExpectedInnerException(IMemberConstraintSet validator, CommandExecutionException exception)
        {
            validator.VerifyThat(() => exception.InnerException)
                .IsInstanceOf<ChallengeAlreadyAcceptedException>()
                .And(ContainsExpectedChallengeId);
        }

        private void ContainsExpectedChallengeId(IMemberConstraintSet validator, ChallengeAlreadyAcceptedException exception)
        {
            validator.VerifyThat(() => exception.ChallengeId).IsEqualTo(Message.ChallengeId);
        }
    }
}