using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Challenges.AcceptChallenge;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.RejectChallenge
{
    [TestClass]
    public sealed class ChallengeAlreadyAcceptedScenario : InMemoryScenario<RejectChallengeCommand>
    {
        public readonly ChallengeIsAcceptedScenario ChallengeIsAccepted;

        public ChallengeAlreadyAcceptedScenario()
        {
            ChallengeIsAccepted = new ChallengeIsAcceptedScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return ChallengeIsAccepted;
        }

        protected override MessageToHandle<RejectChallengeCommand> When()
        {
            var message = new RejectChallengeCommand(ChallengeIsAccepted.ChallengeAcceptedEvent.ChallengeId);
            var receiverId = ChallengeIsAccepted.PlayerIsChallenged.PlayerChallengedEvent.ReceiverId;
            var receiverName = ChallengeIsAccepted.PlayerIsChallenged.ReceiverIsRegistered.PlayerRegisteredEvent.PlayerName;
            return new SecureMessage<RejectChallengeCommand>(message, receiverId, receiverName);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
