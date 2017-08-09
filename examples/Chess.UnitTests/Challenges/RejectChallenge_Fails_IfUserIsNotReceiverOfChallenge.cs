using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Challenges.ChallengePlayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.RejectChallenge
{
    [TestClass]
    public sealed class RejectChallenge_Fails_IfUserIsNotReceiverOfChallenge : UnitTest<RejectChallengeCommand>
    {
        public readonly PlayerIsChallengedScenario PlayerIsChallenged;

        public RejectChallenge_Fails_IfUserIsNotReceiverOfChallenge()
        {
            PlayerIsChallenged = new PlayerIsChallengedScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return PlayerIsChallenged;
        }

        protected override MessageToHandle<RejectChallengeCommand> When()
        {
            var message = new RejectChallengeCommand(PlayerIsChallenged.PlayerChallengedEvent.ChallengeId);
            var session = RandomSession();
            return new SecureMessage<RejectChallengeCommand>(message, session);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
