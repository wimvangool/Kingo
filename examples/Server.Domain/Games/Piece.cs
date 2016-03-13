using System;
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

        public virtual bool CanBeHitByEnPassantMove
        {
            get { return false; }
        }

        public bool HasColor(ColorOfPiece color)
        {
            return Color == color;
        }

        public override string ToString()
        {
            return $"{GetType().Name} ({Color})";
        }

        public void Move(ChessBoard board, Square from, Square to)
        {
            Square enPassantHit = null;

            if (IsSupportedMove(board, from, to, ref enPassantHit))
            {                
                EventBus.Publish(new PieceMovedEvent(from.ToString(), to.ToString())
                {
                    EnPassantHit = enPassantHit?.ToString(),
                    NewState = board.DetermineNewState(from, to, enPassantHit, Color)
                });
                return;
            }
            throw NewUnsupportedMoveException(from, to);
        }          

        protected virtual bool IsSupportedMove(ChessBoard board, Square from, Square to, ref Square enPassantHit)
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
    }
}
