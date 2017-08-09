using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Samples.Chess.Challenges.ChallengePlayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges
{
    [TestClass]
    public sealed class AcceptChallenge_Succeeds_IfChallengeCanBeAccepted : UnitTest<AcceptChallengeCommand>
    {
        public readonly PlayerIsChallengedScenario PlayerIsChallenged;

        public AcceptChallenge_Succeeds_IfChallengeCanBeAccepted()
        {
            PlayerIsChallenged = new PlayerIsChallengedScenario();
        }

        public ChallengeAcceptedEvent ChallengeAcceptedEvent
        {
            get { return (ChallengeAcceptedEvent) PublishedEvents[0]; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return PlayerIsChallenged;
        }

        protected override MessageToHandle<AcceptChallengeCommand> When()
        {
            var message = new AcceptChallengeCommand(PlayerIsChallenged.PlayerChallengedEvent.ChallengeId);
            var receiverId = PlayerIsChallenged.PlayerChallengedEvent.ReceiverId;
            var receiverName = PlayerIsChallenged.ReceiverIsRegistered.PlayerRegisteredEvent.UserName;
            return new SecureMessage<AcceptChallengeCommand>(message, receiverId, receiverName);
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
