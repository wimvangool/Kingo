using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.RejectChallenge
{
    [TestClass]
    public sealed class RejectChallenge_Fails_IfChallengeWasAlreadyAccepted : UnitTest<RejectChallengeCommand>
    {
        public readonly AcceptChallenge_Succeeds_IfChallengeCanBeAccepted ChallengeIsAccepted;

        public RejectChallenge_Fails_IfChallengeWasAlreadyAccepted()
        {
            ChallengeIsAccepted = new AcceptChallenge_Succeeds_IfChallengeCanBeAccepted();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return ChallengeIsAccepted;
        }

        protected override MessageToHandle<RejectChallengeCommand> When()
        {
            var message = new RejectChallengeCommand(ChallengeIsAccepted.ChallengeAcceptedEvent.ChallengeId);
            var receiverId = ChallengeIsAccepted.PlayerIsChallenged.PlayerChallengedEvent.ReceiverId;
            var receiverName = ChallengeIsAccepted.PlayerIsChallenged.ReceiverIsRegistered.PlayerRegisteredEvent.UserName;
            return new SecureMessage<RejectChallengeCommand>(message, receiverId, receiverName);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
