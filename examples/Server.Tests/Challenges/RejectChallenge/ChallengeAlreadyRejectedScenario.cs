using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.RejectChallenge
{
    [TestClass]
    public sealed class ChallengeAlreadyRejectedScenario : MemoryScenario<RejectChallengeCommand>
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

        protected override RejectChallengeCommand When()
        {
            return new RejectChallengeCommand(ChallengeIsRejected.ChallengeRejectedEvent.ChallengeId);
        }

        protected override Session CreateSession()
        {
            var receiverId = ChallengeIsRejected.PlayerIsChallenged.PlayerChallengedEvent.ReceiverId;
            var receiverName = ChallengeIsRejected.PlayerIsChallenged.ReceiverIsRegistered.PlayerRegisteredEvent.PlayerName;

            return new Session(receiverId, receiverName);
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
