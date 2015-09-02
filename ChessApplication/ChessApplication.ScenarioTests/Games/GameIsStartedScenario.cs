using System;
using System.Collections.Generic;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Constraints;
using Kingo.ChessApplication.Challenges;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Games
{
    [TestClass]
    public sealed class GameIsStartedScenario : UnitTestScenario<ChallengeAcceptedEvent>
    {	    
        public readonly ChallengeIsAcceptedScenario ChallengeWasAccepted;

        public GameIsStartedScenario()
        {
            ChallengeWasAccepted = new ChallengeIsAcceptedScenario();
        }

        private Guid SenderId
        {
            get { return ChallengeWasAccepted.PlayerIsChallenged.SenderIsRegistered.PlayerRegisteredEvent.PlayerId; }
        }

        private Guid ReceiverId
        {
            get { return ChallengeWasAccepted.PlayerIsChallenged.ReceiverIsRegistered.PlayerRegisteredEvent.PlayerId; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return ChallengeWasAccepted;
        }

        protected override ChallengeAcceptedEvent When()
        {
            return ChallengeWasAccepted.ChallengeAcceptedEvent.Copy();
        }

        [TestMethod]
        public override void Then()
        {
            VerifyThatDomainEventCount().IsEqualTo(1);
            //VerifyThatDomainEventAtIndex(0).IsInstanceOf<GameStartedEvent>().Any(                         
            //    SenderIsWhiteAndReceiverIsBlack,
            //    SenderIsBlackAndReceiverIsWhite
            //);   
            throw new NotImplementedException();
        }

        private void SenderIsWhiteAndReceiverIsBlack(IMemberConstraintSet<GameStartedEvent> validator)
        {
            validator.VerifyThat(e => e.WhitePlayerId).IsEqualTo(SenderId);
            validator.VerifyThat(e => e.BlackPlayerId).IsEqualTo(ReceiverId);
        }

        private void SenderIsBlackAndReceiverIsWhite(IMemberConstraintSet<GameStartedEvent> validator)
        {
            validator.VerifyThat(e => e.WhitePlayerId).IsEqualTo(ReceiverId);
            validator.VerifyThat(e => e.BlackPlayerId).IsEqualTo(SenderId);
        }
    }
}