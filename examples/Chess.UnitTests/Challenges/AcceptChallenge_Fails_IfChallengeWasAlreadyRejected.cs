using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Samples.Chess.Challenges.RejectChallenge;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges
{
    [TestClass]
    public sealed class AcceptChallenge_Fails_IfChallengeWasAlreadyRejected : UnitTest<AcceptChallengeCommand>
    {
        public readonly RejectChallenge_Succeeds_IfChallengeCanBeRejected RejectChallengeSucceedsIfChallengeCanBeRejected;

        public AcceptChallenge_Fails_IfChallengeWasAlreadyRejected()
        {
            RejectChallengeSucceedsIfChallengeCanBeRejected = new RejectChallenge_Succeeds_IfChallengeCanBeRejected();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return RejectChallengeSucceedsIfChallengeCanBeRejected;
        }

        protected override MessageToHandle<AcceptChallengeCommand> When()
        {
            var message = new AcceptChallengeCommand(RejectChallengeSucceedsIfChallengeCanBeRejected.ChallengeRejectedEvent.ChallengeId);
            var receiverId = RejectChallengeSucceedsIfChallengeCanBeRejected.PlayerIsChallenged.PlayerChallengedEvent.ReceiverId;
            var receiverName = RejectChallengeSucceedsIfChallengeCanBeRejected.PlayerIsChallenged.ReceiverIsRegistered.PlayerRegisteredEvent.UserName;
            return new SecureMessage<AcceptChallengeCommand>(message, receiverId, receiverName);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
