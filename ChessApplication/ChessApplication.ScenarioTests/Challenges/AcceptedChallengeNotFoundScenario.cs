using System;
using System.Collections.Generic;
using Kingo.BuildingBlocks.ComponentModel;
using Kingo.BuildingBlocks.ComponentModel.Server;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Constraints;
using Kingo.BuildingBlocks.Messaging.Domain;
using Kingo.ChessApplication.Players;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Challenges
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
                .IsInstanceOf<AggregateNotFoundByKeyException<Guid>>()
                .And(ContainsExpectedChallengeId);
        }

        private void ContainsExpectedChallengeId(IMemberConstraintSet validator, AggregateNotFoundByKeyException<Guid> exception)
        {
            validator.VerifyThat(() => exception.AggregateKey).IsEqualTo(Message.ChallengeId);
        }
    }
}