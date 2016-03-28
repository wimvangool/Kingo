using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Resources;

namespace Kingo.Samples.Chess.Games
{
    internal abstract class Piece
    {        
        protected abstract IDomainEventBus<Guid, int> EventBus
        {
            get;
        }

        protected abstract ColorOfPiece Color
        {
            get;
        }

        protected abstract TypeOfPiece Type
        {
            get;
        }

        public virtual bool CanTakePartInCastlingMove
        {
            get { return false; }
        }

        public virtual bool CanBeHitByEnPassantMove
        {
            get { return false; }
        }

        public bool HasColor(ColorOfPiece color)
        {
            return Color == color;
        }

        public bool IsOfType(TypeOfPiece type)
        {
            return Type == type;
        }

        public override string ToString()
        {
            return $"{GetType().Name} ({Color})";
        }

        #region [====== Move ======]

        public bool CanMove(ChessBoard board, Square @from)
        {
            var possibleMoves =
                from to in GetPossibleSquaresToMoveTo(@from)
                let @event = SimulateMove(board, Games.Move.Calculate(@from, to))
                where @event != null && @event.NewState != GameState.Error
                select to;

            return possibleMoves.Any();
        }

        protected abstract IEnumerable<Square> GetPossibleSquaresToMoveTo(Square from);

        public void Move(ChessBoard board, Move move)
        {
            var @event = SimulateMove(board, move);
            if (@event == null)
            {
                throw NewUnsupportedMoveException(move);
            }
            if (@event.NewState == GameState.Error)
            {
                throw NewOwnKingLeftInCheckException(move);
            }
            EventBus.Publish(SimulateMove(board, move));            
        }          

        private PieceMovedEvent SimulateMove(ChessBoard board, Move move)
        {
            Func<PieceMovedEvent> eventFactory = () => new PieceMovedEvent(move.From.ToString(), move.To.ToString())
            {
                NewState = board.SimulateMove(move, Color)
            };
            if (IsSupportedMove(board, move, ref eventFactory))
            {
                return eventFactory.Invoke();
            }
            return null;
        }

        public bool IsSupportedMove(ChessBoard board, Move move)
        {
            Func<PieceMovedEvent> eventFactory = null;

            return IsSupportedMove(board, move, ref eventFactory);
        }

        protected virtual bool IsSupportedMove(ChessBoard board, Move move, ref Func<PieceMovedEvent> eventFactory)
        {
            // A piece can never move to a square where another piece of the same color is already present.
            return IsNotPieceOfSameColor(board.SelectPiece(move.To));            
        }

        private bool IsNotPieceOfSameColor(Piece piece)
        {
            return piece == null || !piece.HasColor(Color);
        }

        public virtual Piece RemainInPlace()
        {
            return this;
        }

        public virtual Piece ApplyMove(Move move)
        {
            return this;
        }

        private Exception NewUnsupportedMoveException(Move move)
        {
            var messageFormat = ExceptionMessages.Game_UnsupportedMove;
            var message = string.Format(messageFormat, move.From, move.To, GetType().Name);
            return new DomainException(message);
        }

        private static Exception NewOwnKingLeftInCheckException(Move move)
        {
            var messageFormat = ExceptionMessages.Game_OwnKingLeftInCheck;
            var message = string.Format(messageFormat, move.From, move.To);
            return new DomainException(message);
        }

        #endregion

        #region [====== Pawn Promotion ======]

        public virtual void PromoteTo(ChessBoard board, Square position, TypeOfPiece promoteTo)
        {
            throw NewUnsupportedPromotionException();
        }

        public virtual Piece ApplyPromotion(TypeOfPiece promoteTo)
        {
            throw NewUnsupportedPromotionException();
        }

        private Exception NewUnsupportedPromotionException()
        {
            return new NotSupportedException($"A {Type} cannot be promoted to another piece.");
        }

        #endregion
    }
}
