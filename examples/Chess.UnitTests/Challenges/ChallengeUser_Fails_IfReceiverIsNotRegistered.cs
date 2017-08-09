using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Users.RegisterPlayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.ChallengePlayer
{
    [TestClass]
    public sealed class ChallengeUser_Fails_IfReceiverIsNotRegistered : UnitTest<ChallengeUserCommand>
    {
        public readonly PlayerIsRegisteredScenario PlayerIsRegistered;

        public ChallengeUser_Fails_IfReceiverIsNotRegistered()
        {
            PlayerIsRegistered = new PlayerIsRegisteredScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return PlayerIsRegistered;
        }

        protected override MessageToHandle<ChallengeUserCommand> When()
        {
            var message = new ChallengeUserCommand(Guid.NewGuid(), Guid.NewGuid());
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
