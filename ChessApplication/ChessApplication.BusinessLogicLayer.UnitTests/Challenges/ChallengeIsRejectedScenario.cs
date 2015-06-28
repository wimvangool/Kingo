using System.Collections.Generic;
using System.ComponentModel.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SummerBreeze.ChessApplication.Players;

namespace SummerBreeze.ChessApplication.Challenges
{
    [TestClass]
    public sealed class ChallengeIsRejectedScenario : UnitTestScenario<RejectChallengeCommand>
    {	    
        private readonly PlayerIsChallengedScenario _playerIsChallenged;

        public ChallengeIsRejectedScenario()
        {
            _playerIsChallenged = new PlayerIsChallengedScenario();
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