using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Constraints;
using Kingo.Messaging;
using Kingo.Samples.Chess.Challenges;
using Kingo.Samples.Chess.Challenges.AcceptChallenge;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.ChallengeAccepted
{
    [TestClass]
    public sealed class GameIsStartedScenario : InMemoryScenario<ChallengeAcceptedEvent>
    {        
        public readonly ChallengeIsAcceptedScenario ChallengeIsAccepted;

        public GameIsStartedScenario()
        {
            ChallengeIsAccepted = new ChallengeIsAcceptedScenario();
        }

        public Guid WhitePlayerId
        {
            get { return GameStartedEvent.WhitePlayerId; }
        }

        public string WhitePlayerName
        {
            get { return ChallengeIsAccepted.PlayerIsChallenged.SenderIsRegistered.PlayerRegisteredEvent.PlayerName; }
        }        

        public GameStartedEvent GameStartedEvent
        {
            get { return (GameStartedEvent) PublishedEvents[0]; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return ChallengeIsAccepted;
        }

        protected override ChallengeAcceptedEvent When()
        {
            return ChallengeIsAccepted.ChallengeAcceptedEvent;
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Events().Expect<GameStartedEvent>(Validate).ExecuteAsync();
        }

        private void Validate(IMemberConstraintSet<GameStartedEvent> validator)
        {            
            validator.VerifyThat(m => m.GameId).IsEqualTo(Message.ChallengeId);
            validator.VerifyThat(m => m.GameVersion).IsEqualTo(1);                                   
        }       
    }
}
