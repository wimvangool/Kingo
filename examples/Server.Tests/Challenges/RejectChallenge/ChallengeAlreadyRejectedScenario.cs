using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.RejectChallenge
{
    [TestClass]
    public sealed class ChallengeAlreadyRejectedScenario : InMemoryScenario<RejectChallengeCommand>
    {
        public readonly ChallengeIsRejectedScenario ChallengeIsRejected;

        public ChallengeAlreadyRejectedScenario()
        {
            ChallengeIsRejected = new ChallengeIsRejectedScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return ChallengeIsRejected;
        }

        protected override MessageToHandle<RejectChallengeCommand> When()
        {
            var message = new RejectChallengeCommand(ChallengeIsRejected.ChallengeRejectedEvent.ChallengeId);
            var receiverId = ChallengeIsRejected.PlayerIsChallenged.PlayerChallengedEvent.ReceiverId;
            var receiverName = ChallengeIsRejected.PlayerIsChallenged.ReceiverIsRegistered.PlayerRegisteredEvent.PlayerName;
            return new SecureMessage<RejectChallengeCommand>(message, receiverId, receiverName);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
