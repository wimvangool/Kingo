using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class Queen : ChessPiece
    {
        public Queen(IDomainEventBus<Guid, int> eventBus, ColorOfPiece color)
        {
            EventBus = eventBus;       
            Color = color;
        }

        protected override IDomainEventBus<Guid, int> EventBus
        {
            get;
        }

        protected override ColorOfPiece Color
        {
            get;
        }

        protected override TypeOfPiece Type
        {
            get { return TypeOfPiece.Queen; }
        }

        protected override IEnumerable<Square> GetPossibleSquaresToMoveTo(Square from)
        {
            return Bishop.PossibleSquaresToMoveTo(from).Concat(Rook.PossibleSquaresToMoveTo(from));
        }

        protected override bool IsSupportedMove(ChessBoard board, Move move, ref Func<PieceMovedEvent> eventFactory)
        {
            if (base.IsSupportedMove(board, move, ref eventFactory))
            {
                return (move.IsStraightPath || move.IsCrossPath) && move.IsEmptyPath(board);
            }
            return false;
        }        
    }
}
