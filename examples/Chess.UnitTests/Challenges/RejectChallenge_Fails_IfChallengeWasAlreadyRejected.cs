using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.RejectChallenge
{
    [TestClass]
    public sealed class RejectChallenge_Fails_IfChallengeWasAlreadyRejected : UnitTest<RejectChallengeCommand>
    {
        public readonly RejectChallenge_Succeeds_IfChallengeCanBeRejected RejectChallengeSucceedsIfChallengeCanBeRejected;

        public RejectChallenge_Fails_IfChallengeWasAlreadyRejected()
        {
            RejectChallengeSucceedsIfChallengeCanBeRejected = new RejectChallenge_Succeeds_IfChallengeCanBeRejected();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return RejectChallengeSucceedsIfChallengeCanBeRejected;
        }

        protected override MessageToHandle<RejectChallengeCommand> When()
        {
            var message = new RejectChallengeCommand(RejectChallengeSucceedsIfChallengeCanBeRejected.ChallengeRejectedEvent.ChallengeId);
            var receiverId = RejectChallengeSucceedsIfChallengeCanBeRejected.PlayerIsChallenged.PlayerChallengedEvent.ReceiverId;
            var receiverName = RejectChallengeSucceedsIfChallengeCanBeRejected.PlayerIsChallenged.ReceiverIsRegistered.PlayerRegisteredEvent.UserName;
            return new SecureMessage<RejectChallengeCommand>(message, receiverId, receiverName);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
