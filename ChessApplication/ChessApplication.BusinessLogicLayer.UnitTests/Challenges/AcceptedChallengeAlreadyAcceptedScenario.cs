using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.FluentValidation;
using System.ComponentModel.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SummerBreeze.ChessApplication.Challenges
{
    [TestClass]
    public sealed class AcceptedChallengeAlreadyAcceptedScenario : UnitTestScenario<AcceptChallengeCommand>
    {
        private readonly ChallengeIsAcceptedScenario _challengeIsAccepted;

        public AcceptedChallengeAlreadyAcceptedScenario()
        {
            _challengeIsAccepted = new ChallengeIsAcceptedScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return _challengeIsAccepted;
        }

        protected override AcceptChallengeCommand When()
        {
            return new AcceptChallengeCommand(_challengeIsAccepted.ChallengeAcceptedEvent.ChallengeId);
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