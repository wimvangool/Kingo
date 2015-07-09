﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syztem.ComponentModel;
using Syztem.ComponentModel.FluentValidation;
using Syztem.ComponentModel.Server;

namespace SummerBreeze.ChessApplication.Challenges
{
    [TestClass]
    public sealed class AcceptedChallengeAlreadyRejectedScenario : UnitTestScenario<AcceptChallengeCommand>
    {
        private readonly ChallengeIsRejectedScenario _challengeIsRejected;

        public AcceptedChallengeAlreadyRejectedScenario()
        {
            _challengeIsRejected = new ChallengeIsRejectedScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return _challengeIsRejected;
        }

        protected override AcceptChallengeCommand When()
        {
            return new AcceptChallengeCommand(_challengeIsRejected.ChallengeRejectedEvent.ChallengeId);
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