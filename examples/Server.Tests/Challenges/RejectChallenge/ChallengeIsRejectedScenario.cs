using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Constraints;
using Kingo.Messaging;
using Kingo.Samples.Chess.Challenges.ChallengePlayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.RejectChallenge
{
    [TestClass]
    public sealed class ChallengeIsRejectedScenario : InMemoryScenario<RejectChallengeCommand>
    {
        public readonly PlayerIsChallengedScenario PlayerIsChallenged;

        public ChallengeIsRejectedScenario()
        {
            PlayerIsChallenged = new PlayerIsChallengedScenario();
        }

        public ChallengeRejectedEvent ChallengeRejectedEvent
        {
            get { return (ChallengeRejectedEvent) PublishedEvents[0]; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return PlayerIsChallenged;
        }

        protected override MessageToHandle<RejectChallengeCommand> When()
        {
            var message = new RejectChallengeCommand(PlayerIsChallenged.PlayerChallengedEvent.ChallengeId);
            var receiverId = PlayerIsChallenged.PlayerChallengedEvent.ReceiverId;
            var receiverName = PlayerIsChallenged.ReceiverIsRegistered.PlayerRegisteredEvent.PlayerName;
            return new SecureMessage<RejectChallengeCommand>(message, receiverId, receiverName);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Events().Expect<ChallengeRejectedEvent>(Validate).ExecuteAsync();
        }

        private void Validate(IMemberConstraintSet<ChallengeRejectedEvent> validator)
        {
            validator.VerifyThat(m => m.ChallengeId).IsEqualTo(PlayerIsChallenged.PlayerChallengedEvent.ChallengeId);
            validator.VerifyThat(m => m.ChallengeVersion).IsGreaterThan(PlayerIsChallenged.PlayerChallengedEvent.ChallengeVersion);
        }
    }
}
