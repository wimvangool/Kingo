using System.Collections.Generic;
using Kingo.BuildingBlocks.ComponentModel.Constraints;
using Kingo.BuildingBlocks.ComponentModel.Server;
using Kingo.ChessApplication.Players;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Challenges
{
    [TestClass]
    public sealed class ChallengeIsAcceptedScenario : UnitTestScenario<AcceptChallengeCommand>
    {
        public readonly PlayerIsChallengedScenario PlayerIsChallenged;

        public ChallengeIsAcceptedScenario()
        {
            PlayerIsChallenged = new PlayerIsChallengedScenario();
        }

        internal ChallengeAcceptedEvent ChallengeAcceptedEvent
        {
            get { return GetDomainEventAt<ChallengeAcceptedEvent>(0); }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return PlayerIsChallenged;
        }

        protected override AcceptChallengeCommand When()
        {
            return new AcceptChallengeCommand(PlayerIsChallenged.PlayerChallengedEvent.ChallengeId);
        }

        [TestMethod]
        public override void Then()
        {
            VerifyThatDomainEventCount().IsEqualTo(1);
            VerifyThatDomainEventAtIndex(0).IsInstanceOf<ChallengeAcceptedEvent>().And((validator, @event) =>
            {
                validator.VerifyThat(() => @event.ChallengeId).IsEqualTo(Message.ChallengeId);
            });
        }
    }
}