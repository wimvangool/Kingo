using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Users.RegisterPlayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.ChallengePlayer
{
    [TestClass]
    public sealed class ChallengeUser_Fails_IfSenderIsNotRegistered : UnitTest<ChallengeUserCommand>
    {
        public readonly PlayerIsRegisteredScenario PlayerIsRegistered;

        public ChallengeUser_Fails_IfSenderIsNotRegistered()
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
            var session = new Session(Guid.NewGuid(), "Sender");
            return new SecureMessage<ChallengeUserCommand>(message, session);
        }       

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }

        protected override ChallengeUserCommand WhenMessageIsHandled()
        {
            throw new NotImplementedException();
        }
    }
}
