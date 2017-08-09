using System;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Games
{    
    public sealed class Game : AggregateRoot<Guid, int>
    {        
        private Player _white;
        private Player _black;
        private ChessBoard _board;
        private GameState _state;        

        internal Game(GameStartedEvent @event)
            : base(@event)
        {
            _white = new ActivePlayer(this, @event.WhitePlayerId, ColorOfPiece.White);
            _black = new PassivePlayer(this, @event.BlackPlayerId, ColorOfPiece.Black);
            _board = ChessBoard.SetupNewGame(this);
        }

        internal ChessBoard Board =>
            _board;

        protected override int NextVersion() =>
            Version + 1;

        protected override ISnapshot<Guid, int> TakeSnapshot()
        {
            throw new NotImplementedException();
        }

        #region [====== CommandHandlers ======]

        public void MovePiece(Guid playerId, Square from, Square to)
        {
            if (IsEndState(_state))
            {
                throw NewGameEndedException(Id);
            }
            if (_state == GameState.AwaitingPawnPromotion)
            {
                throw NewAwaitingPawnPromotionException(Id);
            }
            SelectPlayer(playerId).MovePiece(from, to);
        }

        public void PromotePawn(Guid playerId, TypeOfPiece promoteTo)
        {            
            if (_state != GameState.AwaitingPawnPromotion)
            {
                throw NewCannotPromotePawnException(Id);
            }
            SelectPlayer(playerId).PromotePawn(promoteTo);
        }        

        public void Forfeit(Guid playerId)
        {
            if (IsEndState(_state))
            {
                throw NewGameEndedException(Id);
            }
            SelectPlayer(playerId).Forfeit();
        }

        private Player SelectPlayer(Guid playerId)
        {            
            if (_white.IsPlayer(playerId))
            {
                return _white;
            }
            if (_black.IsPlayer(playerId))
            {
                return _black;
            }
            throw NewUnknownPlayerException(Id, playerId);
        }

        private static bool IsEndState(GameState state)
        {
            return
                state == GameState.CheckMate ||
                state == GameState.StaleMate ||
                state == GameState.Forfeited;
        }

        private static Exception NewGameEndedException(Guid gameId)
        {
            var messageFormat = ExceptionMessages.Game_GameEnded;
            var message = string.Format(messageFormat, gameId);
            return new IllegalOperationException(message);
        }

        private static Exception NewAwaitingPawnPromotionException(Guid gameId)
        {
            var messageFormat = ExceptionMessages.Game_AwaitingPawnPromotion;
            var message = string.Format(messageFormat, gameId);
            return new IllegalOperationException(message);
        }

        private static Exception NewCannotPromotePawnException(Guid gameId)
        {
            var messageFormat = ExceptionMessages.Game_CannotPromotePawn;
            var message = string.Format(messageFormat, gameId);
            return new IllegalOperationException(message);
        }

        private static Exception NewUnknownPlayerException(Guid gameId, Guid playerId)
        {
            var messageFormat = ExceptionMessages.Game_UnknownPlayer;
            var message = string.Format(messageFormat, playerId, gameId);
            return new IllegalOperationException(message);
        }

        #endregion

        #region [====== EventHandlers ======]

        protected override EventHandlerCollection RegisterEventHandlers(EventHandlerCollection eventHandlers)
        {
            return eventHandlers
                .Register<PieceMovedEvent>(Handle)
                .Register<EnPassantHitEvent>(Handle)
                .Register<CastlingPerformedEvent>(Handle)
                .Register<PawnMovedToEightRankEvent>(Handle)
                .Register<PawnPromotedEvent>(Handle)
                .Register<GameForfeitedEvent>(Handle);
        }                   

        private void Handle(PieceMovedEvent @event)
        {            
            _white = _white.SwitchTurn();
            _black = _black.SwitchTurn();
            _board = _board.ApplyMove(Square.Parse(@event.From), Square.Parse(@event.To));
            _state = @event.NewState;
        }

        private void Handle(EnPassantHitEvent @event)
        {
            _white = _white.SwitchTurn();
            _black = _black.SwitchTurn();
            _board = _board.ApplyEnPassantMove(Square.Parse(@event.From), Square.Parse(@event.To), Square.Parse(@event.EnPassantHit));
            _state = @event.NewState;
        }

        private void Handle(CastlingPerformedEvent @event)
        {
            _white = _white.SwitchTurn();
            _black = _black.SwitchTurn();
            _board = _board.ApplyCastlingMove(Square.Parse(@event.From), Square.Parse(@event.To), Square.Parse(@event.RookFrom), Square.Parse(@event.RookTo));
            _state = @event.NewState;
        }

        private void Handle(PawnMovedToEightRankEvent @event)
        {
            _board = _board.ApplyMove(Square.Parse(@event.From), Square.Parse(@event.To));
            _state = @event.NewState;
        }

        private void Handle(PawnPromotedEvent @event)
        {
            _white = _white.SwitchTurn();
            _black = _black.SwitchTurn();
            _board = _board.ApplyPawnPromotion(Square.Parse(@event.PawnPosition), @event.PromotedTo);
            _state = @event.NewState;
        }

        private void Handle(GameForfeitedEvent @event)
        {
            _state = GameState.Forfeited;
        }

        #endregion        
    }
}
