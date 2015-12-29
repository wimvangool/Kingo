using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Challenges.RejectChallenge;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.AcceptChallenge
{
    [TestClass]
    public sealed class ChallengeAlreadyRejectedScenario : WriteOnlyScenario<AcceptChallengeCommand>
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

        protected override AcceptChallengeCommand When()
        {
            return new AcceptChallengeCommand(ChallengeIsRejected.ChallengeRejectedEvent.ChallengeId);
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
