using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.FluentValidation;
using System.ComponentModel.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SummerBreeze.ChessApplication.Challenges;
using SummerBreeze.ChessApplication.Players;

namespace SummerBreeze.ChessApplication.Games
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
            VerifyThatDomainEventAtIndex(0).IsInstanceOf<GameStartedEvent>().And((validator, @event) =>
            {                
                validator.VerifyThat(() => @event).Any(
                    SenderIsWhiteAndReceiverIsBlack,
                    SenderIsBlackAndReceiverIsWhite);
            });   
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