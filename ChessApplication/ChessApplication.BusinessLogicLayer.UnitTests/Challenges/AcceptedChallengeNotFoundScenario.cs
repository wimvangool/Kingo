using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.FluentValidation;
using System.ComponentModel.Server;
using System.ComponentModel.Server.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SummerBreeze.ChessApplication.Players;

namespace SummerBreeze.ChessApplication.Challenges
{
    [TestClass]
    public sealed class AcceptedChallengeNotFoundScenario : UnitTestScenario<AcceptChallengeCommand>
    {
        private readonly PlayerIsChallengedScenario _playerIsChallenged;

        public AcceptedChallengeNotFoundScenario()
        {
            _playerIsChallenged = new PlayerIsChallengedScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return _playerIsChallenged;
        }

        protected override AcceptChallengeCommand When()
        {
            return new AcceptChallengeCommand(Guid.NewGuid());
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