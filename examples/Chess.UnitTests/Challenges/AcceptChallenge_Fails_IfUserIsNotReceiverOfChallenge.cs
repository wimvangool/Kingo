using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Challenges.ChallengePlayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges
{
    [TestClass]
    public sealed class AcceptChallenge_Fails_IfUserIsNotReceiverOfChallenge : UnitTest<AcceptChallengeCommand>
    {
        public readonly PlayerIsChallengedScenario PlayerIsChallenged;

        public AcceptChallenge_Fails_IfUserIsNotReceiverOfChallenge()
        {
            PlayerIsChallenged = new PlayerIsChallengedScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return PlayerIsChallenged;
        }

        protected override MessageToHandle<AcceptChallengeCommand> When()
        {
            var message = new AcceptChallengeCommand(PlayerIsChallenged.PlayerChallengedEvent.ChallengeId);
            var session = RandomSession();
            return new SecureMessage<AcceptChallengeCommand>(message, session);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
