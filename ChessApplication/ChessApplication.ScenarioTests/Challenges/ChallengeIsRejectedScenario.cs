using System.Collections.Generic;
using Kingo.BuildingBlocks.ComponentModel.Server;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Constraints;
using Kingo.ChessApplication.Players;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Challenges
{
    [TestClass]
    public sealed class ChallengeIsRejectedScenario : UnitTestScenario<RejectChallengeCommand>
    {	    
        private readonly PlayerIsChallengedScenario _playerIsChallenged;

        public ChallengeIsRejectedScenario()
        {
            _playerIsChallenged = new PlayerIsChallengedScenario();
        }

        internal ChallengeRejectedEvent ChallengeRejectedEvent
        {
            get { return GetDomainEventAt<ChallengeRejectedEvent>(0); }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return _playerIsChallenged;
        }

        protected override RejectChallengeCommand When()
        {
            return new RejectChallengeCommand(_playerIsChallenged.PlayerChallengedEvent.ChallengeId);
        }

        [TestMethod]
        public override void Then()
        {
            VerifyThatDomainEventCount().IsEqualTo(1);
            VerifyThatDomainEventAtIndex(0).IsInstanceOf<ChallengeRejectedEvent>().And((validator, @event) =>
            {
                validator.VerifyThat(() => @event.ChallengeId).IsEqualTo(Message.ChallengeId);
            });
        }
    }
}