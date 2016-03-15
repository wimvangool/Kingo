using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Constraints;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece
{
    [TestClass]
    public abstract class MovePieceScenario : InMemoryScenario<MovePieceCommand>
    {        
        protected MovePieceScenario()
        {
            GameIsStarted = new GameIsStartedScenario();            
        }

        public virtual GameIsStartedScenario GameIsStarted
        {
            get;
        }

        protected Guid GameId
        {
            get { return GameIsStarted.GameStartedEvent.GameId; }
        }               

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return GameIsStarted;
        }

        public SecureMessage<MovePieceCommand> WhitePlayerMove(string from, string to)
        {
            return CreatePlayerMove(from, to, GameIsStarted.WhitePlayerId, GameIsStarted.WhitePlayerName);
        }

        public SecureMessage<MovePieceCommand> BlackPlayerMove(string from, string to)
        {
            return CreatePlayerMove(from, to, GameIsStarted.BlackPlayerId, GameIsStarted.BlackPlayerName);
        }

        private SecureMessage<MovePieceCommand> CreatePlayerMove(string from, string to, Guid playerId, string playerName)
        {
            var message = new MovePieceCommand(GameId, from, to);
            var session = new Session(playerId, playerName);
            return new SecureMessage<MovePieceCommand>(message, session);
        }

        protected async Task ExpectPieceMovedEvent(GameState expectedState = GameState.Normal)
        {
            await Events().Expect<PieceMovedEvent>(v => Validate(v, expectedState)).ExecuteAsync();
        }

        private void Validate(IMemberConstraintSet<PieceMovedEvent> validator, GameState expectedState)
        {
            validator.VerifyThat(m => m.GameId).IsEqualTo(GameId);
            validator.VerifyThat(m => m.GameVersion).IsGreaterThan(0);
            validator.VerifyThat(m => m.From).IsEqualTo(Message.From);
            validator.VerifyThat(m => m.To).IsEqualTo(Message.To);
            validator.VerifyThat(m => m.NewState).IsEqualTo(expectedState);
        }
    }
}
