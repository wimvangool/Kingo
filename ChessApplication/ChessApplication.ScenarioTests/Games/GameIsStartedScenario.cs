using System;
using System.Collections.Generic;
using Kingo.BuildingBlocks.ComponentModel.Constraints;
using Kingo.BuildingBlocks.ComponentModel.Server;
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

        private void SenderIsWhiteAndReceiverIsBlack(IMemberConstraintSet validator, GameStartedEvent @event)
        {
            validator.VerifyThat(() => @event.WhitePlayerId).IsEqualTo(SenderId);
            validator.VerifyThat(() => @event.BlackPlayerId).IsEqualTo(ReceiverId);
        }

        private void SenderIsBlackAndReceiverIsWhite(IMemberConstraintSet validator, GameStartedEvent @event)
        {
            validator.VerifyThat(() => @event.WhitePlayerId).IsEqualTo(ReceiverId);
            validator.VerifyThat(() => @event.BlackPlayerId).IsEqualTo(SenderId);
        }
    }
}