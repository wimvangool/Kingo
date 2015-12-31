using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Challenges.AcceptChallenge;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.RejectChallenge
{
    [TestClass]
    public sealed class ChallengeAlreadyAcceptedScenario : MemoryScenario<RejectChallengeCommand>
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

        protected override RejectChallengeCommand When()
        {
            return new RejectChallengeCommand(ChallengeIsAccepted.ChallengeAcceptedEvent.ChallengeId);
        }

        protected override Session CreateSession()
        {
            var receiverId = ChallengeIsAccepted.PlayerIsChallenged.PlayerChallengedEvent.ReceiverId;
            var receiverName = ChallengeIsAccepted.PlayerIsChallenged.ReceiverIsRegistered.PlayerRegisteredEvent.PlayerName;

            return new Session(receiverId, receiverName);
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
