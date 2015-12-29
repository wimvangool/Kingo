using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Constraints;
using Kingo.Messaging;
using Kingo.Samples.Chess.Challenges.ChallengePlayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.AcceptChallenge
{
    [TestClass]
    public sealed class ChallengeIsAcceptedScenario : WriteOnlyScenario<AcceptChallengeCommand>
    {
        public readonly PlayerIsChallengedScenario PlayerIsChallenged;

        public ChallengeIsAcceptedScenario()
        {
            PlayerIsChallenged = new PlayerIsChallengedScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return PlayerIsChallenged;
        }

        protected override AcceptChallengeCommand When()
        {
            return new AcceptChallengeCommand(PlayerIsChallenged.PlayerChallengedEvent.ChallengeId);
        }

        protected override Session CreateSession()
        {
            var receiverId = PlayerIsChallenged.PlayerChallengedEvent.ReceiverId;
            var receiverName = PlayerIsChallenged.ReceiverIsRegistered.PlayerRegisteredEvent.PlayerName;

            return new Session(receiverId, receiverName);
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Events().Expect<ChallengeAcceptedEvent>(Validate).ExecuteAsync();
        }

        private void Validate(IMemberConstraintSet<ChallengeAcceptedEvent> validator)
        {
            validator.VerifyThat(m => m.ChallengeId).IsEqualTo(PlayerIsChallenged.PlayerChallengedEvent.ChallengeId);
            validator.VerifyThat(m => m.ChallengeVersion).IsGreaterThan(PlayerIsChallenged.PlayerChallengedEvent.ChallengeVersion);
        }
    }
}
