using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges
{
    [TestClass]
    public sealed class ChallengeUser_Succeeds_IfSenderAndReceiverAreValidUsers : UnitTest<ChallengeUserCommand>
    {
        public readonly RegisterUser_Succeeds_IfUserDoesNotYetExist SenderIsRegistered;
        public readonly RegisterUser_Succeeds_IfUserDoesNotYetExist ReceiverIsRegistered;
        public readonly Guid ChallengeId;

        public ChallengeUser_Succeeds_IfSenderAndReceiverAreValidUsers()
        {
            SenderIsRegistered = new RegisterUser_Succeeds_IfUserDoesNotYetExist("Wim");
            ReceiverIsRegistered = new RegisterUser_Succeeds_IfUserDoesNotYetExist("Peter");
            ChallengeId = Guid.NewGuid();
        }        

        protected override IEnumerable<IMessageStream> Given()
        {
            yield return SenderIsRegistered;
            yield return ReceiverIsRegistered;
        }

        protected override ChallengeUserCommand WhenMessageIsHandled() =>
            new ChallengeUserCommand(ChallengeId, Rec);

        protected override MessageToHandle<ChallengeUserCommand> When()
        {
            var message = new ChallengeUserCommand(Guid.NewGuid(), ReceiverIsRegistered.PlayerRegisteredEvent.UserId);
            var sender = SenderIsRegistered.PlayerRegisteredEvent;
            return new SecureMessage<ChallengeUserCommand>(message, sender.UserId, sender.UserName);
        }        

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Events().Expect<PlayerChallengedEvent>(Validate).ExecuteAsync();
        }

        private void Validate(IMemberConstraintSet<PlayerChallengedEvent> validator)
        {
            validator.VerifyThat(m => m.ChallengeId).IsEqualTo(Message.ChallengeId);
            validator.VerifyThat(m => m.ChallengeVersion).IsEqualTo(1);
            validator.VerifyThat(m => m.SenderId).IsEqualTo(SenderIsRegistered.PlayerRegisteredEvent.UserId);
            validator.VerifyThat(m => m.ReceiverId).IsEqualTo(ReceiverIsRegistered.PlayerRegisteredEvent.UserId);
        }
    }
}
