using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.AcceptChallenge
{
    [TestClass]
    public sealed class ChallengeAlreadyAcceptedScenario : InMemoryScenario<AcceptChallengeCommand>
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

        protected override MessageToHandle<AcceptChallengeCommand> When()
        {
            var message = new AcceptChallengeCommand(ChallengeIsAccepted.ChallengeAcceptedEvent.ChallengeId);
            var receiverId = ChallengeIsAccepted.PlayerIsChallenged.PlayerChallengedEvent.ReceiverId;
            var receiverName = ChallengeIsAccepted.PlayerIsChallenged.ReceiverIsRegistered.PlayerRegisteredEvent.PlayerName;            
            return new SecureMessage<AcceptChallengeCommand>(message, receiverId, receiverName);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
