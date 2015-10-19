using System.Collections.Generic;
using Kingo.BuildingBlocks.Constraints;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Challenges
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
            VerifyThatExceptionIsA<CommandExecutionException>().And(v1 =>
            {
                v1.VerifyThat(exception => exception.InnerException).IsInstanceOf<ChallengeAlreadyRejectedException>().And(v2 =>
                {
                    v2.VerifyThat(exception => exception.ChallengeId).IsEqualTo(Message.ChallengeId);
                });
            });
        }        
    }
}