using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Constraints;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Pawns
{
    [TestClass]
    public sealed class PawnPromotedToKnightScenario : InMemoryScenario<PromotePawnCommand>
    {
        public PawnPromotedToKnightScenario()
        {
            PawnMovedToEightRank = new PawnMovedToEightRankWithHitScenario();
        }

        public PawnMovedToEightRankWithHitScenario PawnMovedToEightRank
        {
            get;
        }

        private Guid GameId
        {
            get { return PawnMovedToEightRank.GameIsStarted.GameStartedEvent.GameId; }
        }

        private Guid PlayerId
        {
            get { return PawnMovedToEightRank.GameIsStarted.WhitePlayerId; }
        }

        private string PlayerName
        {
            get { return PawnMovedToEightRank.GameIsStarted.WhitePlayerName; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return PawnMovedToEightRank;
        }

        protected override MessageToHandle<PromotePawnCommand> When()
        {
            var message = new PromotePawnCommand(GameId, TypeOfPiece.Knight);
            var session = new Session(PlayerId, PlayerName);
            return new SecureMessage<PromotePawnCommand>(message, session);
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedEvent<PawnPromotedEvent>(Validate);
        }

        private void Validate(IMemberConstraintSet<PawnPromotedEvent> validator)
        {
            validator.VerifyThat(m => m.GameId).IsEqualTo(GameId);
            validator.VerifyThat(m => m.GameVersion).IsGreaterThan(0);
            validator.VerifyThat(m => m.PromotedTo).IsEqualTo(TypeOfPiece.Knight);
            validator.VerifyThat(m => m.NewState).IsEqualTo(GameState.Normal);
        }
    }
}
