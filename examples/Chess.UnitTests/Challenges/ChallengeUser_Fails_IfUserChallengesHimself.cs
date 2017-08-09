using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.ChallengePlayer
{
    [TestClass]
    public sealed class ChallengeUser_Fails_UserChallengesHimself : UnitTest<ChallengeUserCommand>
    {
        public readonly PlayerIsRegisteredScenario PlayerIsRegistered;

        public ChallengeUser_Fails_UserChallengesHimself()
        {
            PlayerIsRegistered = new PlayerIsRegisteredScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return PlayerIsRegistered;
        }

        protected override MessageToHandle<ChallengeUserCommand> When()
        {
            var message = new ChallengeUserCommand(Guid.NewGuid(), PlayerIsRegistered.PlayerRegisteredEvent.UserId);
            var player = PlayerIsRegistered.PlayerRegisteredEvent;            
            return new SecureMessage<ChallengeUserCommand>(message, player.UserId, player.UserName);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
