using System;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Resources;
using PostSharp.Patterns.Contracts;

namespace Kingo.Samples.Chess.Games
{    
    public sealed class Game : EventStream<Guid, int>
    {
        private Guid _id;
        private int _version;

        private Player _white;
        private Player _black;
        private ChessBoard _board;
        private GameState _state;        

        internal Game(GameStartedEvent @event)
            : base(@event)
        {
            Handle(@event);            
        }

        internal ChessBoard Board
        {
            get { return _board; }
        }

        #region [====== Id & Version ======]

        public override Guid Id
        {
            get { return _id; }
        }

        protected override int Version
        {
            get { return _version; }
            set { _version = value; }
        }

        #endregion

        #region [====== CommandHandlers ======]

        public void MovePiece(Guid playerId, [NotNull] Square from, [NotNull] Square to)
        {
            SelectPlayer(playerId).MovePiece(from, to);
        }

        public void Forfeit(Guid playerId)
        {
            SelectPlayer(playerId).Forfeit();
        }

        private Player SelectPlayer(Guid playerId)
        {
            if (_state != GameState.Normal)
            {
                throw NewGameEndedException(_id);
            }
            if (_white.IsPlayer(playerId))
            {
                return _white;
            }
            if (_black.IsPlayer(playerId))
            {
                return _black;
            }
            throw NewUnknownPlayerException(_id, playerId);
        }

        private static Exception NewGameEndedException(Guid gameId)
        {
            var messageFormat = ExceptionMessages.Game_GameEnded;
            var message = string.Format(messageFormat, gameId);
            return new DomainException(message);
        }

        private static Exception NewUnknownPlayerException(Guid gameId, Guid playerId)
        {
            var messageFormat = ExceptionMessages.Game_UnknownPlayer;
            var message = string.Format(messageFormat, playerId, gameId);
            return new DomainException(message);
        }

        #endregion

        #region [====== EventHandlers ======]

        protected override void RegisterEventHandlers()
        {
            RegisterEventHandler<GameStartedEvent>(Handle);
            RegisterEventHandler<PieceMovedEvent>(Handle);
            RegisterEventHandler<GameForfeitedEvent>(Handle);
        }

        private void Handle(GameStartedEvent @event)
        {
            _id = @event.GameId;
            _version = @event.GameVersion;
            _white = new ActivePlayer(this, @event.WhitePlayerId, ColorOfPiece.White);
            _black = new PassivePlayer(this, @event.BlackPlayerId, ColorOfPiece.Black);
            _board = ChessBoard.SetupNewGame(this);
        }      

        private void Handle(PieceMovedEvent @event)
        {            
            _white = _white.SwitchTurn();
            _black = _black.SwitchTurn();
            _board = _board.ApplyMove(Square.Parse(@event.From), Square.Parse(@event.To), @event.EnPassantHit == null ? null : Square.Parse(@event.EnPassantHit));
            _state = @event.NewState;
        }
  
        private void Handle(GameForfeitedEvent @event)
        {
            _state = GameState.Forfeited;
        }

        #endregion
    }
}
