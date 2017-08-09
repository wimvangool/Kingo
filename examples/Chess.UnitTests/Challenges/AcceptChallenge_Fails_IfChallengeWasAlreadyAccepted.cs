using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges
{
    [TestClass]
    public sealed class AcceptChallenge_Fails_IfChallengeWasAlreadyAccepted : UnitTest<AcceptChallengeCommand>
    {
        public readonly AcceptChallenge_Succeeds_IfChallengeCanBeAccepted ChallengeIsAccepted;

        public AcceptChallenge_Fails_IfChallengeWasAlreadyAccepted()
        {
            ChallengeIsAccepted = new AcceptChallenge_Succeeds_IfChallengeCanBeAccepted();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return ChallengeIsAccepted;
        }

        protected override MessageToHandle<AcceptChallengeCommand> When()
        {
            var message = new AcceptChallengeCommand(ChallengeIsAccepted.ChallengeAcceptedEvent.ChallengeId);
            var receiverId = ChallengeIsAccepted.PlayerIsChallenged.PlayerChallengedEvent.ReceiverId;
            var receiverName = ChallengeIsAccepted.PlayerIsChallenged.ReceiverIsRegistered.PlayerRegisteredEvent.UserName;            
            return new SecureMessage<AcceptChallengeCommand>(message, receiverId, receiverName);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
