using System.Collections.Generic;
using Kingo.BuildingBlocks.Constraints;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Challenges
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
            VerifyThatExceptionIsA<CommandExecutionException>().And(v1 =>
            {
                v1.VerifyThat(exception => exception.InnerException).IsInstanceOf<ChallengeAlreadyAcceptedException>().And(v2 =>
                {
                    v2.VerifyThat(exception => exception.ChallengeId).IsEqualTo(Message.ChallengeId);
                });
            });
        }        
    }
}