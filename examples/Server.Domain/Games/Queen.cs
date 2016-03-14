using System;
using Kingo.Messaging.Domain;

namespace Kingo.Samples.Chess.Games
{
    internal sealed class Queen : Piece
    {
        public Queen(Game game, ColorOfPiece color)
        {
            EventBus = game;       
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

        protected override bool IsSupportedMove(ChessBoard board, Square @from, Square to, ref Func<PieceMovedEvent> eventFactory)
        {
            if (base.IsSupportedMove(board, @from, to, ref eventFactory))
            {
                return IsSupportedMove(board, Square.CalculateMove(from, to));
            }
            return false;
        }

        private static bool IsSupportedMove(ChessBoard board, Move move)
        {
            return (move.IsStraightPath || move.IsCrossPath) && move.IsEmptyPath(board);
        }
    }
}
