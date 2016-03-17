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

        public bool CanMove(ChessBoard board, Square @from)
        {
            var possibleMoves =
                from to in GetPossibleSquaresToMoveTo(@from)
                let @event = SimulateMove(board, @from, to)
                where @event != null && @event.NewState != GameState.Error
                select to;

            return possibleMoves.Any();
        }

        protected abstract IEnumerable<Square> GetPossibleSquaresToMoveTo(Square from);

        public void Move(ChessBoard board, Square from, Square to)
        {
            var @event = SimulateMove(board, from, to);
            if (@event == null)
            {
                throw NewUnsupportedMoveException(from, to);
            }
            if (@event.NewState == GameState.Error)
            {
                throw NewOwnKingLeftInCheckException(from, to);
            }
            EventBus.Publish(SimulateMove(board, from, to));            
        }          

        private PieceMovedEvent SimulateMove(ChessBoard board, Square from, Square to)
        {
            Func<PieceMovedEvent> eventFactory = () => new PieceMovedEvent(from.ToString(), to.ToString())
            {
                NewState = board.SimulateMove(from, to, Color)
            };
            if (IsSupportedMove(board, from, to, ref eventFactory))
            {
                return eventFactory.Invoke();
            }
            return null;
        }

        public bool IsSupportedMove(ChessBoard board, Square from, Square to)
        {
            Func<PieceMovedEvent> eventFactory = null;

            return IsSupportedMove(board, from, to, ref eventFactory);
        }

        protected virtual bool IsSupportedMove(ChessBoard board, Square from, Square to, ref Func<PieceMovedEvent> eventFactory)
        {
            // A piece can never move to a square where another piece of the same color is already present.
            return !to.Equals(from) && IsNotPieceOfSameColor(board.SelectPiece(to));            
        }

        private bool IsNotPieceOfSameColor(Piece piece)
        {
            return piece == null || !piece.HasColor(Color);
        }

        public virtual Piece RemainInPlace()
        {
            return this;
        }

        public virtual Piece ApplyMove(Square from, Square to)
        {
            return this;
        }

        private Exception NewUnsupportedMoveException(Square from, Square to)
        {
            var messageFormat = ExceptionMessages.Game_UnsupportedMove;
            var message = string.Format(messageFormat, @from, to, GetType().Name);
            return new DomainException(message);
        }

        private static Exception NewOwnKingLeftInCheckException(Square from, Square to)
        {
            var messageFormat = ExceptionMessages.Game_OwnKingLeftInCheck;
            var message = string.Format(messageFormat, from, to);
            return new DomainException(message);
        }
    }
}
