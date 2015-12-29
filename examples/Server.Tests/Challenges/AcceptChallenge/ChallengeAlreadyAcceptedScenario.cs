using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.AcceptChallenge
{
    [TestClass]
    public sealed class ChallengeAlreadyAcceptedScenario : WriteOnlyScenario<AcceptChallengeCommand>
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

        protected override AcceptChallengeCommand When()
        {
            return new AcceptChallengeCommand(ChallengeIsAccepted.ChallengeAcceptedEvent.ChallengeId);
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
